using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PbdViewer.Uitils.PbClass
{
	public class PbVariable
	{
		[CompilerGenerated]
		private readonly byte[] _003CBuffer_003Ek__BackingField;

		private string _sqlDeclear;

		public ushort Index { get; set; }

		public ushort GlobalIndex
		{
			get
			{
				if (!IsShared)
				{
					return ushort.MaxValue;
				}
				return BufferHelper.GetUShort(Buffer, 12L);
			}
		}

		public PbType Type { get; set; }

		public PbVariableFlag Flag { get; set; }

		public string PrecisionOrSize { get; set; }

		public string Name { get; set; }

		public string ArrayString { get; set; }

		public string AccessString { get; set; }

		public bool IsReferencedGlobal { get; set; }

		public bool IsShared { get; set; }

		public bool IsInstance { get; set; }

		public bool IsIndirect { get; set; }

		public bool IsConstant { get; set; }

		public PbEntry Entry { get; set; }

		public PbObject Object { get; set; }

		public byte[] Buffer
		{
			[CompilerGenerated]
			get
			{
				return _003CBuffer_003Ek__BackingField;
			}
		}

		public PbVariable(PbEntry pbEntry, ushort index, byte[] buffer, byte[] structBuffer, bool delayParseType)
		{
			Entry = pbEntry;
			_003CBuffer_003Ek__BackingField = buffer;
			Index = index;
			Flag = (PbVariableFlag)Buffer[17];
			AccessString = (Flag.HasFlag(PbVariableFlag.IsPrivate) ? "private " : (Flag.HasFlag(PbVariableFlag.IsProtected) ? "protected " : ""));
			IsShared = Flag.HasFlag(PbVariableFlag.IsShared);
			IsReferencedGlobal = (buffer[16] & 0x40) == 64;
			IsInstance = (buffer[0] & 0xF) <= 1;
			IsIndirect = (buffer[0] & 2) == 2;
			IsConstant = (buffer[0] & 4) == 4;
			if (!delayParseType)
			{
				ParseType();
			}
			Name = BufferHelper.GetString(pbEntry.Project.IsUnicode, structBuffer, (int)BufferHelper.GetUInt(buffer, 8L));
			ArrayString = GetArrayString(BufferHelper.GetUInt(buffer, 4L), structBuffer);
		}

		public PbVariable Inherit(PbObject control)
		{
			PbVariable obj = (PbVariable)MemberwiseClone();
			obj.Type = control.Type;
			obj.Entry = control.Entry;
			obj.Object = control;
			return obj;
		}

		public void ParseType()
		{
			Type = PbType.GetPbType(Entry, BufferHelper.GetUShort(Buffer, 18L));
			if (Type.IsValueType)
			{
				if (Type.Name == "blob")
				{
					ushort uShort = BufferHelper.GetUShort(Buffer, 12L);
					PrecisionOrSize = ((uShort == 0) ? "" : string.Format("{{{0}}}", BufferHelper.GetUShort(Buffer, 12L)));
				}
				else if (Type.Name == "decimal")
				{
					int num = Buffer[16] & 0x3F;
					PrecisionOrSize = ((num == 62) ? "" : string.Format("{{{0}}}", (int)Buffer[16] / 2));
				}
			}
		}

		public static string GetArrayString(uint offset, byte[] buffer)
		{
			string text = string.Empty;
			if (offset != 65535)
			{
				text += "[";
				byte b = buffer[offset];
				for (int i = 0; i < b; i++)
				{
					if (i != 0)
					{
						text += ",";
					}
					uint uInt = BufferHelper.GetUInt(buffer, offset + 4 + i * 8);
					uint uInt2 = BufferHelper.GetUInt(buffer, offset + 8 + i * 8);
					if (uInt == 1)
					{
						text += uInt2;
					}
					else if (uInt != uInt2 || uInt2 != 0)
					{
						text = text + uInt + " to " + uInt2;
					}
				}
				text += "]";
			}
			return text;
		}

		public string GetValue(byte[] valueBuffer)
		{
			if (!Flag.HasFlag(PbVariableFlag.IsCustom))
			{
				return null;
			}
			if (!Type.IsValueType && Type.Enum == null)
			{
				return null;
			}
			if (IsIndirect || IsReferencedGlobal)
			{
				return null;
			}
			if (Flag.HasFlag(PbVariableFlag.IsArray))
			{
				if (Flag.HasFlag(PbVariableFlag.Invalid))
				{
					return null;
				}
				List<uint> list = GetList(valueBuffer);
				List<string> list2 = list.Select((uint o) => GetValue(o, valueBuffer, true)).ToList();
				while (list2.Count > 0 && list2.Last() == null)
				{
					list2.RemoveAt(list2.Count - 1);
				}
				for (int i = 0; i < list2.Count; i++)
				{
					if (list2[i] == null)
					{
						list2[i] = GetValue(list[i], valueBuffer, false);
					}
				}
				return string.Format("{{{0}}}", string.Join(",", list2));
			}
			return GetValue(BufferHelper.GetUInt(Buffer, 12L), valueBuffer, true);
		}

		private string GetValue(uint code, byte[] valueBuffer, bool checkisdefault)
		{
			if (Type.Enum != null)
			{
				if (!checkisdefault || (ushort)code != 0)
				{
					return Type.Enum.Items[(ushort)code];
				}
				return null;
			}
			switch (Type.Name)
			{
			case "integer":
				if (!checkisdefault || (short)code != 0)
				{
					return string.Format("{0}", (short)code);
				}
				return null;
			case "uint":
				if (!checkisdefault || (ushort)code != 0)
				{
					return string.Format("{0}", (ushort)code);
				}
				return null;
			case "long":
				if (!checkisdefault || code != 0)
				{
					return string.Format("{0}", (int)code);
				}
				return null;
			case "ulong":
				if (!checkisdefault || code != 0)
				{
					return string.Format("{0}", code);
				}
				return null;
			case "char":
				if (!checkisdefault || (ushort)code != 0)
				{
					return string.Format("'{0}'", (char)code);
				}
				return null;
			case "byte":
				if (!checkisdefault || (ushort)code != 0)
				{
					return string.Format("{0}", (byte)code);
				}
				return null;
			case "boolean":
				if (!checkisdefault || (byte)code != 0)
				{
					return string.Format("{0}", (byte)code != 0).ToLower();
				}
				return null;
			case "real":
				if (!checkisdefault || code != 0)
				{
					return BufferHelper.GetReal(code);
				}
				return null;
			case "string":
				if (!checkisdefault || (!Flag.HasFlag(PbVariableFlag.Invalid) && !(BufferHelper.GetString(Entry.Project.IsUnicode, valueBuffer, code) == "")))
				{
					return string.Format("{0}", BufferHelper.GetEscapeString(Entry.Project.IsUnicode, valueBuffer, code));
				}
				return null;
			case "decimal":
				if (!checkisdefault || (!Flag.HasFlag(PbVariableFlag.Invalid) && !(BufferHelper.GetDecimal(valueBuffer, code) == "0.0")))
				{
					return string.Format("{0}", BufferHelper.GetDecimal(valueBuffer, code));
				}
				return null;
			case "double":
				if (!checkisdefault || (!Flag.HasFlag(PbVariableFlag.Invalid) && !(BufferHelper.GetDouble(valueBuffer, code) == "0")))
				{
					return BufferHelper.GetDouble(valueBuffer, code);
				}
				return null;
			case "longlong":
				if (!checkisdefault || (!Flag.HasFlag(PbVariableFlag.Invalid) && !(BufferHelper.GetLongLong(valueBuffer, code) == "0")))
				{
					return BufferHelper.GetLongLong(valueBuffer, code);
				}
				return null;
			case "date":
				if (!checkisdefault || (!Flag.HasFlag(PbVariableFlag.Invalid) && !(BufferHelper.GetDate(valueBuffer, code) == "1900-01-01")))
				{
					return string.Format("{0}", BufferHelper.GetDate(valueBuffer, code));
				}
				return null;
			case "time":
				if (!checkisdefault || (!Flag.HasFlag(PbVariableFlag.Invalid) && !(BufferHelper.GetTime(valueBuffer, code) == "00:00:00")))
				{
					return string.Format("{0}", BufferHelper.GetTime(valueBuffer, code));
				}
				return null;
			case "datetime":
				if (!checkisdefault || (!Flag.HasFlag(PbVariableFlag.Invalid) && !(BufferHelper.GetDateTime(valueBuffer, code) == "datetime(1900-01-01,00:00:00)")))
				{
					return string.Format("{0}", BufferHelper.GetDateTime(valueBuffer, code));
				}
				return null;
			default:
				return null;
			}
		}

		private List<uint> GetList(byte[] valuebuffer)
		{
			List<uint> list = new List<uint>();
			ushort uShort = BufferHelper.GetUShort(Buffer, 12L);
			ushort uShort2 = BufferHelper.GetUShort(valuebuffer, uShort + 14);
			int num = uShort + 28 + uShort2 * 8;
			uint uInt = BufferHelper.GetUInt(valuebuffer, num);
			for (int i = 0; i < uInt; i++)
			{
				list.Add(BufferHelper.GetUInt(valuebuffer, num + 4 + 8 * i));
			}
			return list;
		}

		public void SetCursorParams(IEnumerable<string> paramList, string sqlcaStr)
		{
			if (_sqlDeclear == null)
			{
				_sqlDeclear = BufferHelper.GetCursor(Entry.Project.IsUnicode, Entry.VariableBuffer, BufferHelper.GetUInt(Buffer, 12L), paramList);
				_sqlDeclear = string.Format("declare {0} cursor for {1} using {2} ;", Name, _sqlDeclear, sqlcaStr);
			}
		}

		public void SetDynamicCursorParams(string sqlsaStr)
		{
			if (_sqlDeclear == null)
			{
				_sqlDeclear = BufferHelper.GetCursor(Entry.Project.IsUnicode, Entry.VariableBuffer, BufferHelper.GetUInt(Buffer, 12L), null);
				_sqlDeclear = string.Format("declare {0} dynamic cursor {1} for {2} ;", Name, _sqlDeclear, sqlsaStr);
			}
		}

		public void SetProcdureParams(IEnumerable<string> paramList, string sqlcaStr)
		{
			if (_sqlDeclear == null)
			{
				_sqlDeclear = BufferHelper.GetCursor(Entry.Project.IsUnicode, Entry.VariableBuffer, BufferHelper.GetUInt(Buffer, 12L), paramList).Replace("execute ", "");
				_sqlDeclear = string.Format("declare {0} procedure for {1} using {2} ;", Name, _sqlDeclear, sqlcaStr);
			}
		}

		public void SetDynamictProcdureParams(string sqlsaStr)
		{
			if (_sqlDeclear == null)
			{
				_sqlDeclear = BufferHelper.GetCursor(Entry.Project.IsUnicode, Entry.VariableBuffer, BufferHelper.GetUInt(Buffer, 12L), null);
				_sqlDeclear = string.Format("declare {0} dynamic procedure {1} for {2} ;", Name, _sqlDeclear, sqlsaStr);
			}
		}

		public string ToString(byte[] valueBuffer, bool debug = false)
		{
			string text = string.Format("{0}{1}{2} {3}{4}", AccessString, Type.Name, PrecisionOrSize, Name, ArrayString);
			if (_sqlDeclear != null)
			{
				text = _sqlDeclear;
			}
			if (IsConstant)
			{
				text = "constant " + text;
			}
			if (debug)
			{
				if (IsReferencedGlobal)
				{
					text = "global " + text;
				}
				else if (IsShared)
				{
					text = "shared " + text;
				}
				text = BufferHelper.GetHexString(Buffer) + "  " + text;
			}
			if (valueBuffer != null)
			{
				string text2 = null;
				if (!IsReferencedGlobal)
				{
					text2 = GetValue(valueBuffer);
				}
				if (text2 != null)
				{
					text += string.Format(" = {0}", text2);
				}
			}
			return text;
		}
	}
}
