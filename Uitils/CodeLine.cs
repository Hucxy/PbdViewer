using System.Collections.Generic;
using System.Linq;

namespace PbdViewer.Uitils
{
	public class CodeLine
	{
		public ushort PCodePositon;

		public ushort? DebugLine;

		public ushort PCodeOp;

		public byte[] PCodeParam;

		public string SCode;

		public ushort JmpPositon;

		public JmpType JmpType;

		public string Condition;

		public CodeLine PreCodeLine;

		public CodeLine NextCodeLine;

		public readonly List<string> LabelSCode = new List<string>();

		public override string ToString()
		{
			string text = string.Join("", LabelSCode.Select((string o) => string.Format("{0} {1}\r\n", "".PadRight(47), o)));
			string text2 = string.Format("{0:X4}", DebugLine);
			text2 = string.Format("{0} {1:X4}:  ", text2.PadRight(4), PCodePositon) + string.Format("{0:X4}  {1}", PCodeOp, BufferHelper.GetHexString(PCodeParam));
			text2 = string.Format("{0} {1}", text2.PadRight(47), SCode);
			return text + text2;
		}
	}
}
