namespace PbdViewer.DataModel
{
	internal class DirectoryNode : TreeNode
	{
		public DirectoryNode(string name)
		{
			base.Name = name;
			base.NodeType = NodeType.Directory;
		}
	}
}
