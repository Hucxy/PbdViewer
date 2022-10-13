using System.Linq;
using PbdViewer.Uitils.PbClass;

namespace PbdViewer.DataModel
{
	internal class ExternalFunctionsNode : TreeNode
	{
		public ExternalFunctionsNode(string name, PbFunctionDefinition[] pbFunctionDefinitions, bool isAll = false)
		{
			ExternalFunctionsNode externalFunctionsNode = this;
			base.Name = name;
			base.NodeType = NodeType.ExternalFunctions;
			base.Text = string.Join("\r\n", pbFunctionDefinitions.Select((PbFunctionDefinition o) => externalFunctionsNode.ShowVariable(o, isAll)));
		}

		private string ShowVariable(PbFunctionDefinition definition, bool isAll)
		{
			if (definition == null)
			{
				return null;
			}
			string text = definition.ToString();
			if (isAll && definition.Object != null)
			{
				text = string.Format("{0}  // {1} -> {2} -> {3}", text, definition.Object.Type.Name, definition.Object.Entry.EntryName, definition.Object.Entry.File.FileName);
			}
			return text;
		}
	}
}
