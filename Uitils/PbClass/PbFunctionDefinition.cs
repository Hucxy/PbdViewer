using System.Linq;

namespace PbdViewer.Uitils.PbClass
{
	public class PbFunctionDefinition
	{
		public PbObject Object { get; set; }

		public ushort Index { get; set; }

		public ushort GlobalIndex { get; set; }

		public ushort RefIndex { get; set; }

		public ushort EventCode { get; set; }

		public bool IsEvent
		{
			get
			{
				return Flag.HasFlag(PbFunctionFlag.IsEvent);
			}
		}

		public bool IsExternal
		{
			get
			{
				return Flag.HasFlag(PbFunctionFlag.IsExternal);
			}
		}

		public PbFunctionFlag Flag { get; set; }

		public PbType ReturnType { get; set; }

		public string Name { get; set; }

		public PbFunctionParam[] Params { get; set; }

		public string Library { get; set; }

		public string Alias { get; set; }

		public PbType ThrowsType { get; set; }

		public override string ToString()
		{
			string empty = string.Empty;
			if (!IsEvent)
			{
				empty = (Flag.HasFlag(PbFunctionFlag.IsPrivate) ? (empty + "private ") : ((!Flag.HasFlag(PbFunctionFlag.IsProtected)) ? (empty + "public ") : (empty + "protected ")));
				empty = ((ReturnType.Index != 0) ? (empty + "function ") : (empty + "subroutine "));
			}
			else
			{
				empty += "event ";
			}
			empty = empty + ReturnType.Name + " ";
			empty += string.Format("{0}({1})", Name, string.Join(",", Params.Select((PbFunctionParam o) => o.ToString())));
			if (ThrowsType != null)
			{
				empty += string.Format(" throws {0}", ThrowsType.Name);
			}
			if (Library != null)
			{
				empty += string.Format(" library \"{0}\" alias for \"{1}\"", Library, Alias);
			}
			return empty;
		}
	}
}
