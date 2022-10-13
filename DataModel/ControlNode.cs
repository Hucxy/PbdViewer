using System.Linq;
using PbdViewer.Uitils.PbClass;

namespace PbdViewer.DataModel
{
	internal class ControlNode : TreeNode
	{
		public ControlNode(string name, PbObject @object)
		{
			base.Name = ((name == @object.Type.Name) ? name : string.Format("{0}({1})", name, @object.Type.Name));
			PbObject[] controls = @object.Entry.Objects.Values.Where((PbObject o) => o.ParentObject == @object).ToArray();
			PbVariable[] array = @object.Variables.Where((PbVariable o) => !o.IsInstance && controls.FirstOrDefault((PbObject b) => b.Type.Name == o.Type.Name) == null).ToArray();
			if (array.Length != 0)
			{
				base.Children.Add(new VariablesNode("properties", array, @object.Entry.VariableBuffer));
			}
			PbVariable[] allVariables = @object.AllVariables;
			if (allVariables.Length != 0)
			{
				base.Children.Add(new VariablesNode("all instance variables", allVariables, null, true));
			}
			PbFunctionDefinition[] allFunctionDefinitions = @object.AllFunctionDefinitions;
			if (allFunctionDefinitions.Length != 0)
			{
				base.Children.Add(new ExternalFunctionsNode("all functions and events", allFunctionDefinitions, true));
			}
			DirectoryNode directoryNode = new DirectoryNode("controls");
			PbObject[] array2 = controls;
			foreach (PbObject pbObject in array2)
			{
				directoryNode.Children.Add(new ControlNode(pbObject.Type.Name, pbObject));
			}
			if (directoryNode.Children.Count > 0)
			{
				base.Children.Add(directoryNode);
			}
			if (@object.Functions == null)
			{
				return;
			}
			PbFunction[] functions = @object.Functions;
			foreach (PbFunction pbFunction in functions)
			{
				pbFunction.Definition = @object.FunctionDefinitions[pbFunction.Index];
			}
			PbFunction[] array3 = @object.Functions.Where((PbFunction o) => o.Definition.IsEvent).ToArray();
			if (array3.Length != 0)
			{
				DirectoryNode directoryNode2 = new DirectoryNode("events");
				functions = array3;
				foreach (PbFunction pbFunction2 in functions)
				{
					directoryNode2.Children.Add(new FunctionNode(pbFunction2));
				}
				base.Children.Add(directoryNode2);
			}
		}
	}
}
