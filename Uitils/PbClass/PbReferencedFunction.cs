namespace PbdViewer.Uitils.PbClass
{
	public class PbReferencedFunction
	{
		private readonly byte[] _buffer;

		public string Name { get; set; }

		public ushort Index { get; set; }

		public ushort GlobalIndex { get; set; }

		public bool IsGlobalFunction { get; set; }

		public PbReferencedFunction(ushort index, byte[] buffer)
		{
			Index = index;
			_buffer = buffer;
		}

		public string ToString(bool isDebug)
		{
			string text = Name;
			if (isDebug)
			{
				text = string.Format("{0}\t{1:X4}: {2}", BufferHelper.GetHexString(_buffer), GlobalIndex, text);
			}
			return text;
		}
	}
}
