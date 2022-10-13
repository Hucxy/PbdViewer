using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using PbdViewer.DataModel;

namespace PbdViewer.ViewModel
{
	internal class WindowViewModel
	{
		[CompilerGenerated]
		private readonly ObservableCollection<TreeNode> _003CNodes_003Ek__BackingField = new ObservableCollection<TreeNode>();

		public ObservableCollection<TreeNode> Nodes
		{
			[CompilerGenerated]
			get
			{
				return _003CNodes_003Ek__BackingField;
			}
		}

		public TreeNode SelectedNode { get; set; }
	}
}
