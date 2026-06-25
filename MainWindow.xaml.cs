using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using PbdViewer.DataModel;
using PbdViewer.Uitils.PbClass;
using PbdViewer.ViewModel;
using PbdViewer.Properties;
using PbdViewer.Uitils;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace PbdViewer
{
	public partial class MainWindow : Window
	{
		private readonly WindowViewModel _model;
		private readonly List<string> _openFilePaths = new List<string>();
		private bool _isInitializing = true;
		private readonly List<SearchHit> _searchHits = new List<SearchHit>();
		private int _searchIndex = -1;
		private string _lastKeyword;
		private bool _lastIgnoreCase;
		private bool _lastUseRegex;
		private readonly List<TreeNode> _searchPath = new List<TreeNode>();

		private class SearchHit
		{
			public TreeNode Node;

			public List<TreeNode> Ancestors;
		}

		public MainWindow()
		{
			this.InitializeComponent();
			TextEncodingOptions.SetByName(Settings.Default.TextEncoding);
			base.DataContext = (this._model = new WindowViewModel());
			base.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			base.Loaded += this.MainWindow_Loaded;
			this.InitEncodingComboBox();
			this._isInitializing = false;
		}

		private void InitEncodingComboBox()
		{
			this.EncodingComboBox.Items.Clear();
			foreach (string name in TextEncodingOptions.EncodingNames)
			{
				this.EncodingComboBox.Items.Add(name);
			}
			int num = TextEncodingOptions.EncodingNames.IndexOf(Settings.Default.TextEncoding);
			this.EncodingComboBox.SelectedIndex = ((num >= 0) ? num : 0);
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
		}

		private void MainWindow_OnPreviewDragOver(object sender, DragEventArgs e)
		{
			e.Effects = DragDropEffects.None;
			if (!e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				return;
			}
			Array array = (Array)e.Data.GetData(DataFormats.FileDrop);
			string text = (array != null) ? array.GetValue(0).ToString() : null;
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			string a = Path.GetExtension(text).ToLower();
			if (a == ".exe" || a == ".dll" || a == ".pbd" || a == ".pbl")
			{
				e.Effects = DragDropEffects.Move;
			}
			e.Handled = true;
		}

		private void MainWindow_OnPreviewDrop(object sender, DragEventArgs e)
		{
			e.Handled = true;
			if (!e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				return;
			}
			Array array = (Array)e.Data.GetData(DataFormats.FileDrop);
			string text = (array != null) ? array.GetValue(0).ToString() : null;
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			this.AddFile(text);
		}

		private void AddFile(string filename)
		{
			if (!this._openFilePaths.Contains(filename, StringComparer.OrdinalIgnoreCase))
			{
				this._openFilePaths.Add(filename);
			}
			try
			{
				using (List<PbFile>.Enumerator enumerator = new PbProject(filename).Files.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PbFile pbFile = enumerator.Current;
						TreeNode treeNode = this._model.Nodes.FirstOrDefault((TreeNode o) => string.Compare(o.Name, pbFile.FilePath, StringComparison.OrdinalIgnoreCase) == 0);
						if (treeNode != null)
						{
							this._model.Nodes.Remove(treeNode);
						}
						this._model.Nodes.Add(new FileNode(pbFile));
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void EncodingComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this._isInitializing)
			{
				return;
			}
			string text = this.EncodingComboBox.SelectedItem as string;
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			Settings.Default.TextEncoding = text;
			try
			{
				Settings.Default.Save();
			}
			catch
			{
			}
			TextEncodingOptions.SetByName(text);
			this.ReloadAll();
		}

		private void ReloadAll()
		{
			if (this._openFilePaths.Count == 0)
			{
				return;
			}
			List<string> list = new List<string>(this._openFilePaths);
			this._model.Nodes.Clear();
			foreach (string path in list)
			{
				this.AddFile(path);
			}
		}

		private void SearchBox_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				this.FindNext();
			}
		}

		private void SearchNextButton_OnClick(object sender, RoutedEventArgs e)
		{
			this.FindNext();
		}

		private void SearchPrevButton_OnClick(object sender, RoutedEventArgs e)
		{
			this.FindPrev();
		}

		private void EnsureSearched()
		{
			string keyword = this.SearchBox.Text ?? string.Empty;
			bool ignoreCase = !this.CaseCheckBox.IsChecked.GetValueOrDefault();
			bool useRegex = this.RegexCheckBox.IsChecked.GetValueOrDefault();
			if (keyword == this._lastKeyword && ignoreCase == this._lastIgnoreCase && useRegex == this._lastUseRegex)
			{
				return;
			}
			this.PerformSearch(keyword, ignoreCase, useRegex);
		}

		private void FindNext()
		{
			this.EnsureSearched();
			int count = this._searchHits.Count;
			if (count == 0)
			{
				return;
			}
			this._searchIndex = (this._searchIndex + 1 + count) % count;
			this.SelectHit(this._searchIndex);
		}

		private void FindPrev()
		{
			this.EnsureSearched();
			int count = this._searchHits.Count;
			if (count == 0)
			{
				return;
			}
			if (this._searchIndex < 0)
			{
				this._searchIndex = count - 1;
			}
			else
			{
				this._searchIndex = (this._searchIndex - 1 + count) % count;
			}
			this.SelectHit(this._searchIndex);
		}

		private void PerformSearch(string keyword, bool ignoreCase, bool useRegex)
		{
			this._lastKeyword = keyword;
			this._lastIgnoreCase = ignoreCase;
			this._lastUseRegex = useRegex;
			this._searchHits.Clear();
			this._searchIndex = -1;
			if (string.IsNullOrEmpty(keyword))
			{
				this.SearchStatus.Text = "请输入关键字";
				return;
			}
			Regex regex = null;
			if (useRegex)
			{
				try
				{
					regex = new Regex(keyword, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
				}
				catch (ArgumentException)
				{
					this.SearchStatus.Text = "正则语法错误";
					return;
				}
			}
			StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
			this._searchPath.Clear();
			foreach (TreeNode root in this._model.Nodes)
			{
				this.SearchDfs(root, keyword, comparison, regex);
			}
			int count = this._searchHits.Count;
			this.SearchStatus.Text = ((count == 0) ? "无匹配" : ("共 " + count + " 个匹配"));
		}

		private void SearchDfs(TreeNode node, string keyword, StringComparison comparison, Regex regex)
		{
			if (this.MatchesNode(node, keyword, comparison, regex))
			{
				this._searchHits.Add(new SearchHit
				{
					Node = node,
					Ancestors = new List<TreeNode>(this._searchPath)
				});
			}
			this._searchPath.Add(node);
			foreach (TreeNode child in node.Children)
			{
				this.SearchDfs(child, keyword, comparison, regex);
			}
			this._searchPath.RemoveAt(this._searchPath.Count - 1);
		}

		private bool MatchesNode(TreeNode node, string keyword, StringComparison comparison, Regex regex)
		{
			string content = (node.Name ?? string.Empty) + "\n" + (node.Text ?? string.Empty);
			if (regex != null)
			{
				return regex.IsMatch(content);
			}
			return content.IndexOf(keyword, comparison) >= 0;
		}

		private void SelectHit(int index)
		{
			if (index < 0 || index >= this._searchHits.Count)
			{
				return;
			}
			SearchHit searchHit = this._searchHits[index];
			this.SelectAndReveal(searchHit.Node, searchHit.Ancestors);
			this.SearchStatus.Text = (index + 1) + "/" + this._searchHits.Count;
		}

		private void SelectAndReveal(TreeNode node, List<TreeNode> ancestors)
		{
			ItemsControl itemsControl = this.MainTreeView;
			foreach (TreeNode ancestor in ancestors)
			{
				itemsControl.UpdateLayout();
				TreeViewItem treeViewItem = itemsControl.ItemContainerGenerator.ContainerFromItem(ancestor) as TreeViewItem;
				if (treeViewItem == null)
				{
					return;
				}
				treeViewItem.IsExpanded = true;
				treeViewItem.UpdateLayout();
				itemsControl = treeViewItem;
			}
			itemsControl.UpdateLayout();
			TreeViewItem treeViewItem2 = itemsControl.ItemContainerGenerator.ContainerFromItem(node) as TreeViewItem;
			if (treeViewItem2 != null)
			{
				treeViewItem2.IsSelected = true;
				treeViewItem2.BringIntoView();
			}
		}

		private void ExportButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (this._model.Nodes.Count == 0)
			{
				MessageBox.Show("没有可导出的内容，请先打开文件。");
				return;
			}
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";
			saveFileDialog.DefaultExt = ".txt";
			if (this._openFilePaths.Count > 0)
			{
				saveFileDialog.FileName = Path.GetFileNameWithoutExtension(this._openFilePaths[0]) + "_dump.txt";
			}
			bool? result = saveFileDialog.ShowDialog();
			if (result != true)
			{
				return;
			}
			try
			{
				this.ExportAll(saveFileDialog.FileName);
				MessageBox.Show("导出完成：" + saveFileDialog.FileName);
			}
			catch (Exception ex)
			{
				MessageBox.Show("导出失败：" + ex.Message);
			}
		}

		private void ExportAll(string filePath)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("导出文件清单（共 " + this._model.Nodes.Count + " 个；含主文件及其自动加载的依赖库）：");
			foreach (TreeNode root in this._model.Nodes)
			{
				stringBuilder.Append('\t').AppendLine(root.Name ?? string.Empty);
			}
			foreach (TreeNode root in this._model.Nodes)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("================================================================================");
				stringBuilder.AppendLine("[文件] " + (root.Name ?? string.Empty));
				stringBuilder.AppendLine("--------------------------------------------------------------------------------");
				foreach (TreeNode child in root.Children)
				{
					this.ExportNode(stringBuilder, child, 0);
				}
			}
			File.WriteAllText(filePath, stringBuilder.ToString(), new UTF8Encoding(false));
		}

		private void ExportNode(StringBuilder sb, TreeNode node, int depth)
		{
			sb.Append('\t', depth);
			sb.AppendLine(node.Name ?? string.Empty);
			if (!string.IsNullOrEmpty(node.Text))
			{
				string text = node.Text.Replace("\r\n", "\n").Replace("\r", "\n");
				foreach (string line in text.Split('\n'))
				{
					sb.Append('\t', depth + 1);
					sb.AppendLine(line);
				}
			}
			foreach (TreeNode child in node.Children)
			{
				this.ExportNode(sb, child, depth + 1);
			}
		}

		private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			TreeNode treeNode = e.NewValue as TreeNode;
			this.Image.Visibility = Visibility.Collapsed;
			this.RichTextBox.Visibility = Visibility.Collapsed;
			if (treeNode != null)
			{
				if (treeNode.Bitmap != null)
				{
					this.Image.Source = treeNode.Bitmap;
					this.Image.Visibility = Visibility.Visible;
					return;
				}
				this.RichTextBox.Document.Blocks.Clear();
				this.RichTextBox.Document.Blocks.Add(new Paragraph(new Run(treeNode.Text)));
				this.RichTextBox.Visibility = Visibility.Visible;
			}
		}

		private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
		{
			TreeView treeView = (TreeView)sender;
			if (e.Key == Key.Delete)
			{
				TreeNode treeNode = treeView.SelectedItem as TreeNode;
				if (treeNode != null && treeNode.NodeType == NodeType.File)
				{
					this._model.Nodes.Remove(treeNode);
				}
			}
		}
	}
}
