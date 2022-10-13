namespace PbdViewer.Uitils.PbClass
{
	public class PbFunctionParam
	{
		public bool IsReadOnly { get; set; }

		public bool IsReference { get; set; }

		public PbType Type { get; set; }

		public string Name { get; set; }

		public string ArrayString { get; set; }

		public override string ToString()
		{
			string text = string.Empty;
			if (IsReference)
			{
				text += "ref ";
			}
			if (IsReadOnly)
			{
				text += "readonly ";
			}
			return text + string.Format("{0} {1}{2}", Type.Name, Name, ArrayString);
		}
	}
}
