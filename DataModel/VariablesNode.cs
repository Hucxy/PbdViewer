using System.Linq;
using PbdViewer.Uitils.PbClass;

namespace PbdViewer.DataModel
{
	internal class VariablesNode : TreeNode
	{
		public VariablesNode(string name, PbVariable[] variables, byte[] valueBuffer, bool isAll = false)
		{
			VariablesNode variablesNode = this;
			base.Name = name;
			base.NodeType = NodeType.Variables;
			if (variables != null)
			{
				base.Text = string.Join("\r\n", variables.Select((PbVariable o) => variablesNode.ShowVariable(o, valueBuffer, isAll)));
			}
		}

		private string ShowVariable(PbVariable variable, byte[] valueBuffer, bool isAll)
		{
			if (variable == null)
			{
				return null;
			}
			string text = variable.ToString(valueBuffer);
			if (isAll && variable.Object != null)
			{
				text = string.Format("{0}// {1} -> {2} -> {3}", text.PadRight(40), variable.Object.Type.Name, variable.Object.Entry.EntryName, variable.Object.Entry.File.FileName);
			}
			return text;
		}
	}
}
