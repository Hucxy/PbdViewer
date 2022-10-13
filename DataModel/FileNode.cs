using System.Linq;
using PbdViewer.Uitils;
using PbdViewer.Uitils.PbClass;

namespace PbdViewer.DataModel
{
	public class FileNode : TreeNode
	{
		public FileNode(PbFile pbFile)
		{
			base.IsExpanded = true;
			base.Name = pbFile.FileName;
			base.NodeType = NodeType.File;
			ushort version = pbFile.Project.Version;
			int num = PCodeHelper.Count[version];
			int num2 = PCodeHelper.Goodcount[version];
			foreach (IGrouping<string, PbEntry> item in from o in pbFile.Entries
				group o by o.Suffix)
			{
				DirectoryNode directoryNode = new DirectoryNode(item.Key)
				{
					IsExpanded = true
				};
				foreach (PbEntry item2 in item.OrderBy((PbEntry o) => o.Name))
				{
					directoryNode.Children.Add(new EntryNode(item2));
				}
				base.Children.Add(directoryNode);
			}
		}
	}
}
