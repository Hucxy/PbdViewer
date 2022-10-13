using System.Collections.Generic;
using System.Linq;
using PbdViewer.Uitils;
using PbdViewer.Uitils.PbClass;

namespace PbdViewer.DataModel
{
	public class FunctionNode : TreeNode
	{
		public FunctionNode(PbFunction pbFunction)
		{
			base.NodeType = NodeType.Function;
			base.Name = pbFunction.Definition.Name;
			base.Text = string.Concat(base.Text, "//", pbFunction.Definition, "\r\n\r\n");
			IEnumerable<string> values = PCodeHelper.ParsePCode(pbFunction, true);
			for (int i = 0; i < pbFunction.Variables.Length; i++)
			{
				PbVariable variable = pbFunction.Variables[i];
				if (pbFunction.Project.IsDebug)
				{
					base.Text += string.Format("{0:X4}:  {1}\r\n", i, variable.ToString(pbFunction.Buffer, true));
				}
				else if (pbFunction.Definition.Params.FirstOrDefault((PbFunctionParam b) => b.Name == variable.Name) == null && !variable.IsReferencedGlobal && !variable.Name.StartsWith("\u0001"))
				{
					base.Text += string.Format("{0}\r\n", variable.ToString(pbFunction.Buffer));
				}
			}
			if (pbFunction.Project.IsDebug)
			{
				base.Text += "\r\n\r\n";
				for (int j = 0; j < (pbFunction.Buffer.Length + 15) / 16; j++)
				{
					base.Text += string.Format("{0:X4}:  {1}\r\n", j, BufferHelper.GetHexString(pbFunction.Buffer, j * 16, 16));
				}
			}
			base.Text += "\r\n\r\n";
			base.Text += string.Join("\r\n", values);
		}
	}
}
