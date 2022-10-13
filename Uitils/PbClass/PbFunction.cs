using System.Runtime.CompilerServices;

namespace PbdViewer.Uitils.PbClass
{
	public class PbFunction
	{
		[CompilerGenerated]
		private readonly PbProject _003CProject_003Ek__BackingField;

		[CompilerGenerated]
		private readonly PbEntry _003CEntry_003Ek__BackingField;

		[CompilerGenerated]
		private readonly PbObject _003CObject_003Ek__BackingField;

		public PbProject Project
		{
			[CompilerGenerated]
			get
			{
				return _003CProject_003Ek__BackingField;
			}
		}

		public PbEntry Entry
		{
			[CompilerGenerated]
			get
			{
				return _003CEntry_003Ek__BackingField;
			}
		}

		public PbObject Object
		{
			[CompilerGenerated]
			get
			{
				return _003CObject_003Ek__BackingField;
			}
		}

		public ushort Index { get; set; }

		public PbFunctionDefinition Definition { get; set; }

		public byte[] PCodeBytes { get; set; }

		public byte[] DebugBytes { get; set; }

		public byte[] Buffer { get; set; }

		public PbVariable[] Variables { get; set; }

		public PbFunction(PbObject @object)
		{
			_003CObject_003Ek__BackingField = @object;
			_003CEntry_003Ek__BackingField = Object.Entry;
			_003CProject_003Ek__BackingField = Entry.Project;
		}

		public override string ToString()
		{
			PbObject @object = Object;
			PbFunctionDefinition definition = Definition;
			return string.Concat(@object, "/", ((definition != null) ? definition.Name : null) ?? string.Format("#{0:X4}", Index));
		}
	}
}
