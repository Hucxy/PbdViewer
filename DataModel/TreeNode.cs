using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace PbdViewer.DataModel
{
	public class TreeNode
	{
		[CompilerGenerated]
		private readonly ObservableCollection<TreeNode> _003CChildren_003Ek__BackingField = new ObservableCollection<TreeNode>();

		public NodeType NodeType { get; set; }

		public bool IsExpanded { get; set; }

		public bool IsSelected { get; set; }

		public string Name { get; protected set; }

		public string Text { get; protected set; }

		public BitmapImage Bitmap { get; protected set; }

		public ObservableCollection<TreeNode> Children
		{
			[CompilerGenerated]
			get
			{
				return _003CChildren_003Ek__BackingField;
			}
		}
	}
}
