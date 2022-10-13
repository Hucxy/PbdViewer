using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PbdViewer.Uitils.PbClass
{
	public class PbObject
	{
		private bool _parsedInherit;

		[CompilerGenerated]
		private readonly PbEntry _003CEntry_003Ek__BackingField;

		[CompilerGenerated]
		private readonly PbProject _003CProject_003Ek__BackingField;

		[CompilerGenerated]
		private readonly ushort _003CIndex_003Ek__BackingField;

		[CompilerGenerated]
		private readonly PbType _003CType_003Ek__BackingField;

		public PbEntry Entry
		{
			[CompilerGenerated]
			get
			{
				return _003CEntry_003Ek__BackingField;
			}
		}

		public PbProject Project
		{
			[CompilerGenerated]
			get
			{
				return _003CProject_003Ek__BackingField;
			}
		}

		public ushort Index
		{
			[CompilerGenerated]
			get
			{
				return _003CIndex_003Ek__BackingField;
			}
		}

		public PbType Type
		{
			[CompilerGenerated]
			get
			{
				return _003CType_003Ek__BackingField;
			}
		}

		public PbType InheritType { get; set; }

		public PbObject InheritObject { get; set; }

		public PbType ParentType { get; set; }

		public PbObject ParentObject { get; set; }

		public PbFunction[] Functions { get; set; }

		public PbVariable[] Variables { get; set; }

		public PbReferencedFunction[] ReferencedFunctions { get; set; }

		public PbFunctionDefinition[] FunctionDefinitions { get; set; }

		public PbVariable[] AllVariables { get; set; }

		public PbFunctionDefinition[] AllFunctionDefinitions { get; set; }

		public PbObject[] Controls { get; set; }

		public PbObject(PbEntry entry, ushort index, PbType type)
		{
			_003CEntry_003Ek__BackingField = entry;
			_003CIndex_003Ek__BackingField = index;
			_003CType_003Ek__BackingField = type;
			_003CProject_003Ek__BackingField = Entry.Project;
		}

		public void ParseInherit()
		{
			if (_parsedInherit)
			{
				return;
			}
			PbType inheritType = InheritType;
			InheritObject = ((inheritType != null) ? inheritType.GetObject(Entry) : null);
			PbType parentType = ParentType;
			ParentObject = ((parentType != null) ? parentType.GetObject(Entry) : null);
			PbFunctionDefinition[] allFunctionDefinitions;
			if (InheritObject != null)
			{
				InheritObject.ParseInherit();
				int num = Math.Min(InheritObject.AllVariables.Length, AllVariables.Length);
				for (int i = 0; i < num; i++)
				{
					AllVariables[i] = InheritObject.AllVariables[i];
				}
				allFunctionDefinitions = InheritObject.AllFunctionDefinitions;
				foreach (PbFunctionDefinition pbFunctionDefinition in allFunctionDefinitions)
				{
					if (pbFunctionDefinition != null)
					{
						AllFunctionDefinitions[pbFunctionDefinition.GlobalIndex] = pbFunctionDefinition;
					}
				}
			}
			List<PbVariable> list = Variables.Where((PbVariable o) => o.IsInstance).Reverse().ToList();
			for (int k = 0; k < list.Count; k++)
			{
				AllVariables[AllVariables.Length - 1 - k] = list[k];
			}
			allFunctionDefinitions = FunctionDefinitions;
			foreach (PbFunctionDefinition pbFunctionDefinition2 in allFunctionDefinitions)
			{
				AllFunctionDefinitions[pbFunctionDefinition2.GlobalIndex] = pbFunctionDefinition2;
			}
			Controls = Entry.Objects.Values.Where((PbObject o) => o.ParentType == Type).ToArray();
			for (int l = 0; l < AllVariables.Length; l++)
			{
				PbVariable variable = AllVariables[l];
				if (variable != null)
				{
					PbObject pbObject = Controls.FirstOrDefault((PbObject o) => o.Type.Name == variable.Name);
					if (pbObject != null && pbObject.Type != variable.Type)
					{
						AllVariables[l] = variable.Inherit(pbObject);
					}
				}
			}
			_parsedInherit = true;
		}

		public override string ToString()
		{
			return string.Concat(Entry, "/", Type.Name);
		}
	}
}
