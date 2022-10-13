using System.Linq;
using PbdViewer.Uitils.PbClass;

namespace PbdViewer.DataModel
{
	internal class StructureNode : TreeNode
	{
		public StructureNode(PbObject oldPbObject)
		{
			base.Name = oldPbObject.Type.Name;
			base.NodeType = NodeType.Structure;
			if (oldPbObject.Variables != null)
			{
				base.Text = string.Join("\r\n", oldPbObject.Variables.Select((PbVariable o) => o.ToString(null)));
			}
		}
	}
}
