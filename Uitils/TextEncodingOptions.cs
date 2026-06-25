using System;
using System.Collections.Generic;
using System.Text;

namespace PbdViewer.Uitils
{
	public static class TextEncodingOptions
	{
		private static Encoding _ansi = GetEncodingSafe(936);

		public static Encoding Ansi
		{
			get
			{
				return _ansi;
			}
		}

		public static readonly IList<string> EncodingNames = new List<string>
		{
			"GBK",
			"UTF-8",
			"Shift-JIS",
			"Big5",
			"EUC-KR",
			"System Default"
		};

		public static void SetByName(string name)
		{
			Encoding encoding = GetByName(name);
			if (encoding != null)
			{
				_ansi = encoding;
			}
		}

		public static Encoding GetByName(string name)
		{
			int codepage;
			switch (name)
			{
				case "GBK":
					codepage = 936;
					break;
				case "UTF-8":
					codepage = 65001;
					break;
				case "Shift-JIS":
					codepage = 932;
					break;
				case "Big5":
					codepage = 950;
					break;
				case "EUC-KR":
					codepage = 949;
					break;
				case "System Default":
					return Encoding.Default;
				default:
					return null;
			}
			return GetEncodingSafe(codepage);
		}

		private static Encoding GetEncodingSafe(int codepage)
		{
			try
			{
				return Encoding.GetEncoding(codepage);
			}
			catch (NotSupportedException)
			{
				return Encoding.Default;
			}
			catch (ArgumentException)
			{
				return Encoding.Default;
			}
		}
	}
}
