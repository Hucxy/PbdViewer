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

namespace PbdViewer
{
	public partial class MainWindow : Window
	{
		private readonly WindowViewModel _model;

		public MainWindow()
		{
			this.InitializeComponent();
			base.DataContext = (this._model = new WindowViewModel());
			base.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			base.Loaded += this.MainWindow_Loaded;
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
