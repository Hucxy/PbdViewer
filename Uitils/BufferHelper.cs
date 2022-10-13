using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace PbdViewer.Uitils
{
	public static class BufferHelper
	{
		public static byte[] GetBuffer(byte[] buffer, long offset, int size)
		{
			offset &= 0x7FFFFFFF;
			size = Math.Min(size, buffer.Length - (int)offset);
			byte[] array = new byte[size];
			Buffer.BlockCopy(buffer, (int)offset, array, 0, size);
			return array;
		}

		public static string GetHexString(byte[] buffer)
		{
			return string.Join(" ", buffer.Select((byte o) => string.Format("{0:X2}", o)));
		}

		public static string GetHexString(byte[] buffer, long offset, int size)
		{
			return GetHexString(GetBuffer(buffer, offset, size));
		}

		public static ushort GetUShort(byte[] buffer, long offset)
		{
			offset &= 0x7FFFFFFF;
			return (ushort)((buffer[offset + 1] << 8) + buffer[offset]);
		}

		public static uint GetUInt(byte[] buffer, long offset)
		{
			offset &= 0x7FFFFFFF;
			return (uint)((buffer[offset + 3] << 24) + (buffer[offset + 2] << 16) + (buffer[offset + 1] << 8) + buffer[offset]);
		}

		public static string GetDate(byte[] buffer, long offset)
		{
			offset &= 0x7FFFFFFF;
			ushort uShort = GetUShort(buffer, offset + 4);
			return string.Format("{0}-{1:D2}-{2:D2}", uShort + 1900, buffer[offset + 6] + 1, buffer[offset + 7]);
		}

		public static string GetDateTime(byte[] buffer, long offset)
		{
			offset &= 0x7FFFFFFF;
			return string.Format("datetime({0},{1})", GetDate(buffer, offset), GetTime(buffer, offset));
		}

		public static string GetTime(byte[] buffer, long offset)
		{
			offset &= 0x7FFFFFFF;
			string text = string.Format("{0:D2}:{1:D2}:{2:D2}", buffer[offset + 8], buffer[offset + 9], buffer[offset + 10]);
			uint num = GetUInt(buffer, offset) / 1000u;
			if (num != 0)
			{
				text += string.Format(".{0:D3}", num);
			}
			return text;
		}

		public static string GetEscapeString(bool isUnicode, byte[] buffer, long offset)
		{
			string arg = GetString(isUnicode, buffer, offset).Replace("~", "~~").Replace("\r", "~r").Replace("\n", "~n")
				.Replace("\t", "~t")
				.Replace("\"", "~\"");
			return string.Format("\"{0}\"", arg);
		}

		public static string GetString(bool isUnicode, byte[] buffer, long offset)
		{
			offset &= 0x7FFFFFFF;
			long num = offset;
			if (isUnicode)
			{
				for (; num < buffer.Length && (buffer[num] != 0 || buffer[num + 1] != 0); num += 2)
				{
				}
			}
			else
			{
				for (; num < buffer.Length && buffer[num] != 0; num++)
				{
				}
			}
			if (num - offset == 0L)
			{
				return string.Empty;
			}
			if (!isUnicode)
			{
				return Encoding.Default.GetString(buffer, (int)offset, (int)(num - offset));
			}
			return Encoding.Unicode.GetString(buffer, (int)offset, (int)(num - offset));
		}

		public static string GetDecimal(byte[] buffer, long offset)
		{
			offset &= 0x7FFFFFFF;
			ushort uShort = GetUShort(buffer, offset);
			byte b = buffer[offset + 2];
			string text = (new BigInteger(GetUInt(buffer, offset + 4)) + (new BigInteger(GetUInt(buffer, offset + 8)) << 32) + (new BigInteger(GetUShort(buffer, offset + 12)) << 64)).ToString();
			if (b > 0)
			{
				if (text.Length <= b)
				{
					text = text.PadLeft(b + 1, '0');
				}
				text = text.Insert(text.Length - b, ".").TrimEnd('0');
				if (text.EndsWith("."))
				{
					text += "0";
				}
			}
			if (uShort > 0)
			{
				text = "-" + text;
			}
			return text;
		}

		public static string GetReal(uint code)
		{
			return string.Format("{0}", BitConverter.ToSingle(new byte[4]
			{
				(byte)(code & 0xFFu),
				(byte)((code >> 8) & 0xFFu),
				(byte)((code >> 16) & 0xFFu),
				(byte)((code >> 24) & 0xFFu)
			}, 0));
		}

		public static string GetDouble(byte[] buffer, long offset)
		{
			offset &= 0x7FFFFFFF;
			return string.Format("{0}", BitConverter.ToDouble(buffer, (int)offset));
		}

		public static string GetLongLong(byte[] buffer, long offset)
		{
			offset &= 0x7FFFFFFF;
			return string.Format("{0}", BitConverter.ToInt64(buffer, (int)offset));
		}

		public static string GetCursor(bool isUnicode, byte[] bytes, uint offset, IEnumerable<string> paramList)
		{
			uint num = offset & 0x7FFFFFFFu;
			if (GetUInt(bytes, num + 8) != 65535)
			{
				return GetCursor(isUnicode, bytes, GetUInt(bytes, num + 8), paramList);
			}
			string @string = GetString(isUnicode, bytes, GetUInt(bytes, num + 24));
			string text = "";
			if (paramList != null)
			{
				uint num2 = GetUInt(bytes, num + 16);
				int num3 = 0;
				foreach (string param in paramList)
				{
					ushort uShort = GetUShort(bytes, num2);
					ushort uShort2 = GetUShort(bytes, num2 + 2);
					num2 += 4;
					if (uShort == 0 && uShort2 == 0)
					{
						break;
					}
					text = text + @string.Substring(num3, uShort - num3) + string.Format(":{0}", param);
					num3 = uShort2;
				}
				text += @string.Substring(num3);
			}
			return text;
		}
	}
}
