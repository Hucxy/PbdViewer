using System;
using System.Linq;
using PbdViewer.Uitils.PbClass;

namespace PbdViewer.DataModel
{
	public class EntryNode : TreeNode
	{
		private readonly PbEntry _pbEntry;

		public EntryNode(PbEntry pbEntry)
		{
			_pbEntry = pbEntry;
			base.NodeType = NodeType.Node;
			base.Name = pbEntry.EntryName;
			base.Text = pbEntry.Source;
			base.Bitmap = pbEntry.Bitmap;
			switch (pbEntry.Suffix)
			{
			case "apl":
				base.NodeType = NodeType.Application;
				ParseApplication();
				break;
			case "men":
				base.NodeType = NodeType.Menu;
				ParseControl();
				break;
			case "win":
				base.NodeType = NodeType.Window;
				ParseControl();
				break;
			case "udo":
				base.NodeType = NodeType.UserObject;
				ParseControl();
				break;
			case "str":
				base.NodeType = NodeType.Structure;
				ParseStructure();
				break;
			case "fun":
				base.NodeType = NodeType.Function;
				ParseFunction();
				break;
			case "dwo":
				base.NodeType = NodeType.DataWindow;
				break;
			}
		}

		private void ParseStructure()
		{
			base.Children.Add(new VariablesNode("properties", _pbEntry.EntryObject.Variables, null));
		}

		private void ParseFunction()
		{
			if (_pbEntry.EntryObject.Functions.Length != 0)
			{
				DirectoryNode directoryNode = new DirectoryNode("functions");
				PbFunction[] functions = _pbEntry.EntryObject.Functions;
				foreach (PbFunction pbFunction in functions)
				{
					pbFunction.Definition = _pbEntry.EntryObject.FunctionDefinitions[pbFunction.Index];
				}
				foreach (PbFunction item in _pbEntry.EntryObject.Functions.OrderBy((PbFunction o) => o.Definition.Name))
				{
					directoryNode.Children.Add(new FunctionNode(item));
				}
				base.Children.Add(directoryNode);
			}
			PbObject[] array = _pbEntry.Objects.Values.Where(delegate(PbObject o)
			{
				PbType inheritType = o.InheritType;
				return ((inheritType != null) ? inheritType.Name : null) == "structure";
			}).ToArray();
			if (array.Length == 0)
			{
				return;
			}
			DirectoryNode directoryNode2 = new DirectoryNode("structures");
			foreach (PbObject item2 in array.OrderBy((PbObject o) => o.Type.Name))
			{
				directoryNode2.Children.Add(new StructureNode(item2));
			}
			base.Children.Add(directoryNode2);
		}

		private void ParseApplication()
		{
			string name = base.Name.Substring(0, base.Name.IndexOf(".", StringComparison.Ordinal));
			PbVariable[] array = _pbEntry.Variables.Where((PbVariable o) => !o.IsShared && o.Type.Name != name && o.Flag.HasFlag(PbVariableFlag.IsCustom)).ToArray();
			if (array.Length != 0)
			{
				base.Children.Add(new VariablesNode("global variables", array, _pbEntry.VariableBuffer));
			}
			PbFunctionDefinition[] array2 = _pbEntry.EntryObject.FunctionDefinitions.Where((PbFunctionDefinition o) => o.IsExternal).ToArray();
			if (array2.Length != 0)
			{
				base.Children.Add(new ExternalFunctionsNode("global external functions", array2));
			}
			PbVariable[] array3 = _pbEntry.Variables.Where((PbVariable o) => o.IsShared).ToArray();
			if (array3.Length != 0)
			{
				base.Children.Add(new VariablesNode("shared variables", array3, _pbEntry.VariableBuffer));
			}
			PbVariable[] array4 = _pbEntry.EntryObject.Variables.Where((PbVariable o) => o.IsInstance && o.IsShared).ToArray();
			if (array4.Length != 0)
			{
				base.Children.Add(new VariablesNode("instance variables", array4, _pbEntry.VariableBuffer));
			}
			PbVariable[] array5 = _pbEntry.EntryObject.Variables.Where((PbVariable o) => !o.IsInstance).ToArray();
			if (array5.Length != 0)
			{
				base.Children.Add(new VariablesNode("properties", array5, _pbEntry.VariableBuffer));
			}
			PbVariable[] allVariables = _pbEntry.EntryObject.AllVariables;
			if (allVariables.Length != 0)
			{
				base.Children.Add(new VariablesNode("all instance variables", allVariables, null, true));
			}
			PbFunctionDefinition[] allFunctionDefinitions = _pbEntry.EntryObject.AllFunctionDefinitions;
			if (allFunctionDefinitions.Length != 0)
			{
				base.Children.Add(new ExternalFunctionsNode("all functions and events", allFunctionDefinitions, true));
			}
			PbObject[] array6 = _pbEntry.Objects.Values.Where(delegate(PbObject o)
			{
				PbType inheritType = o.InheritType;
				return ((inheritType != null) ? inheritType.Name : null) == "structure";
			}).ToArray();
			if (array6.Length != 0)
			{
				DirectoryNode directoryNode = new DirectoryNode("structures");
				PbObject[] array7 = array6;
				foreach (PbObject oldPbObject in array7)
				{
					directoryNode.Children.Add(new StructureNode(oldPbObject));
				}
				base.Children.Add(directoryNode);
			}
			PbFunction[] functions = _pbEntry.EntryObject.Functions;
			foreach (PbFunction pbFunction in functions)
			{
				pbFunction.Definition = _pbEntry.EntryObject.FunctionDefinitions[pbFunction.Index];
			}
			PbFunction[] array8 = _pbEntry.EntryObject.Functions.Where((PbFunction o) => o.Definition.IsEvent).ToArray();
			if (array8.Length != 0)
			{
				DirectoryNode directoryNode2 = new DirectoryNode("events");
				functions = array8;
				foreach (PbFunction pbFunction2 in functions)
				{
					directoryNode2.Children.Add(new FunctionNode(pbFunction2));
				}
				base.Children.Add(directoryNode2);
			}
			PbFunction[] array9 = _pbEntry.EntryObject.Functions.Where((PbFunction o) => !o.Definition.IsEvent && !o.Definition.IsExternal).ToArray();
			if (array9.Length == 0)
			{
				return;
			}
			DirectoryNode directoryNode3 = new DirectoryNode("functions");
			foreach (PbFunction item in array9.OrderBy((PbFunction o) => o.Definition.Name))
			{
				directoryNode3.Children.Add(new FunctionNode(item));
			}
			base.Children.Add(directoryNode3);
		}

		private void ParseControl()
		{
			PbObject[] array = _pbEntry.Objects.Values.Where(delegate(PbObject o)
			{
				PbType inheritType = o.InheritType;
				return ((inheritType != null) ? inheritType.Name : null) == "structure";
			}).ToArray();
			PbObject[] controls = _pbEntry.Objects.Values.Where((PbObject o) => o.ParentObject == _pbEntry.EntryObject).ToArray();
			PbVariable[] array2 = _pbEntry.Variables.Where((PbVariable o) => o.IsShared).ToArray();
			if (array2.Length != 0)
			{
				base.Children.Add(new VariablesNode("shared variables", array2, _pbEntry.VariableBuffer));
			}
			PbVariable[] array3 = _pbEntry.EntryObject.Variables.Where((PbVariable o) => o.IsInstance && o.IsShared).ToArray();
			if (array3.Length != 0)
			{
				base.Children.Add(new VariablesNode("instance variables", array3, _pbEntry.VariableBuffer));
			}
			PbVariable[] array4 = _pbEntry.EntryObject.Variables.Where((PbVariable o) => !o.IsInstance && controls.FirstOrDefault((PbObject b) => b.Type.Name == o.Type.Name) == null).ToArray();
			if (array4.Length != 0)
			{
				base.Children.Add(new VariablesNode("properties", array4, _pbEntry.VariableBuffer));
			}
			PbVariable[] allVariables = _pbEntry.EntryObject.AllVariables;
			if (allVariables.Length != 0)
			{
				base.Children.Add(new VariablesNode("all instance variables", allVariables, null, true));
			}
			PbFunctionDefinition[] allFunctionDefinitions = _pbEntry.EntryObject.AllFunctionDefinitions;
			if (allFunctionDefinitions.Length != 0)
			{
				base.Children.Add(new ExternalFunctionsNode("all functions and events", allFunctionDefinitions, true));
			}
			PbFunctionDefinition[] array5 = _pbEntry.EntryObject.FunctionDefinitions.Where((PbFunctionDefinition o) => o.IsExternal).ToArray();
			if (array5.Length != 0)
			{
				base.Children.Add(new ExternalFunctionsNode("external functions", array5));
			}
			if (array.Length != 0)
			{
				DirectoryNode directoryNode = new DirectoryNode("structures");
				foreach (PbObject item in array.OrderBy((PbObject o) => o.Type.Name))
				{
					directoryNode.Children.Add(new StructureNode(item));
				}
				base.Children.Add(directoryNode);
			}
			DirectoryNode directoryNode2 = new DirectoryNode("controls");
			foreach (PbObject item2 in controls.OrderBy((PbObject o) => o.Type.Name))
			{
				directoryNode2.Children.Add(new ControlNode(item2.Type.Name, item2));
			}
			if (directoryNode2.Children.Count > 0)
			{
				base.Children.Add(directoryNode2);
			}
			PbFunction[] functions = _pbEntry.EntryObject.Functions;
			foreach (PbFunction pbFunction in functions)
			{
				pbFunction.Definition = _pbEntry.EntryObject.FunctionDefinitions[pbFunction.Index];
			}
			PbFunction[] array6 = _pbEntry.EntryObject.Functions.Where((PbFunction o) => o.Definition.IsEvent).ToArray();
			if (array6.Length != 0)
			{
				DirectoryNode directoryNode3 = new DirectoryNode("events");
				functions = array6;
				foreach (PbFunction pbFunction2 in functions)
				{
					directoryNode3.Children.Add(new FunctionNode(pbFunction2));
				}
				base.Children.Add(directoryNode3);
			}
			PbFunction[] array7 = _pbEntry.EntryObject.Functions.Where((PbFunction o) => !o.Definition.IsEvent && !o.Definition.IsExternal).ToArray();
			if (array7.Length == 0)
			{
				return;
			}
			DirectoryNode directoryNode4 = new DirectoryNode("functions");
			foreach (PbFunction item3 in array7.OrderBy((PbFunction o) => o.Definition.Name))
			{
				directoryNode4.Children.Add(new FunctionNode(item3));
			}
			base.Children.Add(directoryNode4);
		}
	}
}
