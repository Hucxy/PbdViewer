using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PbdViewer.Uitils.PbClass
{
	public class PbEnum
	{
		[CompilerGenerated]
		private readonly Dictionary<ushort, string> _003CItems_003Ek__BackingField = new Dictionary<ushort, string>();

		public ushort Index { get; set; }

		public string Name { get; set; }

		public Dictionary<ushort, string> Items
		{
			[CompilerGenerated]
			get
			{
				return _003CItems_003Ek__BackingField;
			}
		}
	}
}
