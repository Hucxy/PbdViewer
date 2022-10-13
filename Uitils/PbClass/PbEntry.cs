using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media.Imaging;

namespace PbdViewer.Uitils.PbClass
{
	public class PbEntry
	{
		private class IndentBlock : IDisposable
		{
			private readonly PbEntry _pbEntry;

			public IndentBlock(PbEntry pbEntry, string blockName)
			{
				_pbEntry = pbEntry;
				_pbEntry.PrintString(blockName + ": {");
				_pbEntry._indent++;
			}

			public void Dispose()
			{
				_pbEntry._indent--;
				_pbEntry.PrintString("}");
			}
		}

		private readonly byte[] _entryData;

		private bool _isParsed;

		private ushort _flag;

		private int _indent;

		private byte[] _dataBuffer;

		private long _position;

		private readonly StringBuilder _sb = new StringBuilder();

		private bool _isDebug = true;

		[CompilerGenerated]
		private readonly Dictionary<ushort, PbType> _003CTypes_003Ek__BackingField = new Dictionary<ushort, PbType>();

		[CompilerGenerated]
		private readonly Dictionary<ushort, PbObject> _003CObjects_003Ek__BackingField = new Dictionary<ushort, PbObject>();

		[CompilerGenerated]
		private readonly PbProject _003CProject_003Ek__BackingField;

		[CompilerGenerated]
		private readonly PbFile _003CFile_003Ek__BackingField;

		[CompilerGenerated]
		private readonly string _003CEntryName_003Ek__BackingField;

		[CompilerGenerated]
		private readonly string _003CName_003Ek__BackingField;

		[CompilerGenerated]
		private readonly string _003CSuffix_003Ek__BackingField;

		public byte[] VariableBuffer { get; set; }

		private byte[] FunctionBuffer { get; set; }

		private byte[] ParamBuffer { get; set; }

		public Dictionary<ushort, PbType> Types
		{
			[CompilerGenerated]
			get
			{
				return _003CTypes_003Ek__BackingField;
			}
		}

		public PbVariable[] Variables { get; private set; }

		public Dictionary<ushort, PbObject> Objects
		{
			[CompilerGenerated]
			get
			{
				return _003CObjects_003Ek__BackingField;
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

		public PbFile File
		{
			[CompilerGenerated]
			get
			{
				return _003CFile_003Ek__BackingField;
			}
		}

		public PbObject EntryObject { get; private set; }

		public string EntryName
		{
			[CompilerGenerated]
			get
			{
				return _003CEntryName_003Ek__BackingField;
			}
		}

		public string Source { get; private set; }

		public BitmapImage Bitmap { get; private set; }

		public string Name
		{
			[CompilerGenerated]
			get
			{
				return _003CName_003Ek__BackingField;
			}
		}

		public string Suffix
		{
			[CompilerGenerated]
			get
			{
				return _003CSuffix_003Ek__BackingField;
			}
		}

		public DateTime ModifiedTime { get; set; }

		public DateTime CompliedTime { get; set; }

		private void PrintString(string str)
		{
			for (int i = 0; i < _indent; i++)
			{
				_sb.Append("\t");
			}
			_sb.Append(str);
			_sb.Append("\r\n");
		}

		private void PrintBuffer(byte[] buff, int step)
		{
			int num = buff.Length / step;
			for (int i = 0; i < num; i++)
			{
				PrintString(string.Format("{0:X04}:{1:X04}   ", i, i * step) + BufferHelper.GetHexString(buff, i * step, step));
			}
			int num2 = buff.Length - num * step;
			if (num2 > 0)
			{
				PrintString(string.Format("{0:X04}:{1:X04}   ", num, num * step) + BufferHelper.GetHexString(buff, num * step, num2));
			}
		}

		private static DateTime GetTime(uint timeStamp)
		{
			DateTime dateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
			TimeSpan value = new TimeSpan((long)timeStamp * 10000000L);
			return dateTime.Add(value);
		}

		public PbEntry(PbFile file, string entryName, byte[] entryData)
		{
			_entryData = entryData;
			_003CFile_003Ek__BackingField = file;
			_003CEntryName_003Ek__BackingField = entryName;
			_003CProject_003Ek__BackingField = File.Project;
			_003CName_003Ek__BackingField = EntryName.Substring(0, EntryName.LastIndexOfAny(new char[1] { '.' }));
			_003CSuffix_003Ek__BackingField = EntryName.Substring(EntryName.LastIndexOfAny(new char[1] { '.' }) + 1);
			switch (Suffix)
			{
			case "ico":
			case "jpg":
			case "png":
			case "bmp":
				Bitmap = new BitmapImage();
				Bitmap.BeginInit();
				Bitmap.StreamSource = new MemoryStream(_entryData);
				Bitmap.EndInit();
				_isParsed = true;
				break;
			case "exe":
				ParseExe();
				_isParsed = true;
				break;
			case "srj":
				ParseSrj();
				_isParsed = true;
				break;
			case "grp":
				Project.OnSystemEntry(this);
				ParseObject(true);
				_isParsed = true;
				break;
			case "apl":
			case "str":
			case "fun":
			case "win":
			case "men":
			case "udo":
				Project.OnSystemLibrary(BufferHelper.GetUShort(_entryData, 0L));
				break;
			case "dwo":
				Source = "DataWindow可以通过PB接口函数导出";
				_isParsed = true;
				break;
			default:
				Source = Project.GetString(_entryData);
				_isParsed = true;
				break;
			}
		}

		public void ParseObject(bool isSystem = false)
		{
			if (_isParsed)
			{
				return;
			}
			_sb.Clear();
			_dataBuffer = _entryData;
			_position = 0L;
			PrintString(string.Format("Pdb Version: {0:X}", ReadUShort()));
			_flag = ReadUShort();
			PrintString(string.Format("Flag: {0:X}", _flag));
			uint num = ReadUInt();
			PrintString(string.Format("EntryType: {0:X}", num));
			uint num2 = ReadUInt();
			PrintString(string.Format("Unkown: {0:X}", num2));
			ModifiedTime = GetTime(ReadUInt());
			if (Project.Version >= 334)
			{
				ReadUInt();
			}
			PrintString(string.Format("Last Modify Time: {0}", ModifiedTime));
			CompliedTime = GetTime(ReadUInt());
			if (Project.Version >= 334)
			{
				ReadUInt();
			}
			PrintString(string.Format("Last Complied Time: {0}", CompliedTime));
			uint num3 = ReadUInt();
			PrintString(string.Format("Unkown: {0:X}", num3));
			ushort num4 = ReadUShort();
			byte[][] array = new byte[num4][];
			for (int i = 0; i < num4; i++)
			{
				array[i] = ReadBuffer(12);
			}
			VariableBuffer = ReadStructBuffer();
			Variables = ReadVariables(true);
			ushort num5 = ReadUShort();
			ushort num6 = ReadUShort();
			FunctionBuffer = ReadStructBuffer();
			ParamBuffer = ReadStructBuffer();
			using (new IndentBlock(this, "Types"))
			{
				ReadTypes(isSystem);
			}
			PbVariable[] variables = Variables;
			for (int j = 0; j < variables.Length; j++)
			{
				variables[j].ParseType();
			}
			using (new IndentBlock(this, "Globel and Shared Variables"))
			{
				for (int k = 0; k < Variables.Length; k++)
				{
					PrintString(string.Format("{0:X2}:  ", k) + Variables[k].ToString(VariableBuffer, _isDebug));
				}
			}
			PbVariable[] array2 = ReadVariables();
			using (new IndentBlock(this, "Enums"))
			{
				for (int l = 0; l < array2.Length; l++)
				{
					PrintString(string.Format("{0:X2}:  ", l) + array2[l].ToString(null, _isDebug));
					Project.OnNewEnumItem(array2[l].Type, BufferHelper.GetUShort(array2[l].Buffer, 12L), array2[l].Name);
				}
			}
			int size = (Project.IsPb5 ? 8 : 16);
			byte[][] array3 = new byte[num5][];
			for (int m = 0; m < num5; m++)
			{
				array3[m] = ReadBuffer(size);
			}
			byte[][] array4 = new byte[num6][];
			for (int n = 0; n < num6; n++)
			{
				array4[n] = ReadBuffer(32);
			}
			int num7 = 0;
			using (new IndentBlock(this, string.Format("Objects: {0}", num5)))
			{
				for (ushort num8 = 0; num8 < num5; num8 = (ushort)(num8 + 1))
				{
					byte[] array5 = array3[num8];
					PbType pbType = PbType.GetPbType(this, BufferHelper.GetUShort(array5, 2L));
					PbObject pbObject = new PbObject(this, num8, pbType);
					int num9 = (array5[0] >> 1) & 7;
					if (num9 == 0)
					{
						byte[] array6 = array4[num7++];
						pbObject.InheritType = PbType.GetPbType(this, BufferHelper.GetUShort(array6, 0L));
						pbObject.ParentType = PbType.GetPbType(this, BufferHelper.GetUShort(array6, 2L));
						Objects[pbObject.Type.Index] = pbObject;
						if (isSystem)
						{
							Project.OnNewObject(pbObject);
						}
						else if (pbObject.Type.Name == Name)
						{
							EntryObject = pbObject;
							Project.OnNewObject(pbObject);
						}
						else if (!pbObject.Type.Name.Contains('`'))
						{
							Project.OnNewObject(pbObject, Name + "`" + pbObject.Type.Name);
						}
						using (new IndentBlock(this, string.Format("Object[{0}] {1}:{2}", num8, pbType.Name, pbObject.InheritType.Name)))
						{
							PrintBuffer(array5, 16);
							PrintBuffer(array6, array6.Length);
							ReadObject(pbObject, array6);
						}
					}
					else
					{
						using (new IndentBlock(this, string.Format("Object[{0}] {1}", num8, pbType.Name)))
						{
							PrintBuffer(array5, 16);
							switch (num9)
							{
							case 1:
							{
								ushort uShort = BufferHelper.GetUShort(array5, 4L);
								byte[][] array7 = new byte[uShort][];
								for (int num10 = 0; num10 < uShort; num10++)
								{
									array7[num10] = ReadBuffer(8);
									PrintBuffer(array7[num10], 16);
								}
								goto end_IL_0509;
							}
							case 6:
								break;
							default:
								goto end_IL_0509;
							}
							break;
							end_IL_0509:;
						}
					}
				}
			}
			if (Project.IsDebug)
			{
				Source = _sb.ToString();
			}
			_sb.Clear();
			if (_position != _dataBuffer.Length)
			{
				throw new Exception("读取错误");
			}
			_isParsed = true;
		}

		private void ReadObject(PbObject pbObject, byte[] buffer)
		{
			ushort num = ReadUShort();
			pbObject.Functions = new PbFunction[num];
			using (new IndentBlock(this, string.Format("Functions: {0}", num)))
			{
				byte[][] array = new byte[num][];
				for (int i = 0; i < num; i++)
				{
					array[i] = ReadBuffer(4);
				}
				for (int j = 0; j < num; j++)
				{
					pbObject.Functions[j] = new PbFunction(pbObject);
					ReadFunction(pbObject.Functions[j], array[j]);
				}
			}
			num = BufferHelper.GetUShort(buffer, 24L);
			ReadBuffer(6 * num);
			num = BufferHelper.GetUShort(buffer, 22L);
			ReadBuffer(4 * num);
			pbObject.ReferencedFunctions = ReadReferencedFunctions();
			using (new IndentBlock(this, string.Format("Referenced Functions And Events: {0}", pbObject.ReferencedFunctions.Length)))
			{
				for (int k = 0; k < pbObject.ReferencedFunctions.Length; k++)
				{
					PrintString(string.Format("{0:X2}:  ", k) + pbObject.ReferencedFunctions[k].ToString(_isDebug));
				}
			}
			pbObject.Variables = ReadVariables();
			using (new IndentBlock(this, string.Format("Properties Or Controls: {0}", pbObject.Variables.Length)))
			{
				for (int l = 0; l < pbObject.Variables.Length; l++)
				{
					pbObject.Variables[l].Object = pbObject;
					PrintString(string.Format("{0:X2}:  ", l) + pbObject.Variables[l].ToString(VariableBuffer, _isDebug));
				}
			}
			ushort uShort = BufferHelper.GetUShort(buffer, 28L);
			ReadBuffer(8 * uShort);
			pbObject.AllVariables = new PbVariable[uShort];
			int num2 = (Project.IsPb5 ? 12 : 16);
			num = BufferHelper.GetUShort(buffer, 26L);
			byte[] buff = ReadBuffer(num2 * num);
			PrintBuffer(buff, num2);
			num = BufferHelper.GetUShort(buffer, 4L);
			int num3 = ((Project.Version > 146) ? 48 : (Project.IsPb5 ? 32 : 44));
			pbObject.FunctionDefinitions = new PbFunctionDefinition[num];
			pbObject.AllFunctionDefinitions = new PbFunctionDefinition[BufferHelper.GetUShort(buffer, 16L)];
			using (new IndentBlock(this, "Events And Functions"))
			{
				for (ushort num4 = 0; num4 < num; num4 = (ushort)(num4 + 1))
				{
					byte[] array2 = ReadBuffer(num3);
					PrintBuffer(array2, num3);
					PbFunctionDefinition pbFunctionDefinition = new PbFunctionDefinition();
					pbObject.FunctionDefinitions[num4] = pbFunctionDefinition;
					pbFunctionDefinition.Object = pbObject;
					pbFunctionDefinition.Index = num4;
					pbFunctionDefinition.Flag = (PbFunctionFlag)array2[Project.IsPb5 ? 27 : 31];
					pbFunctionDefinition.ReturnType = PbType.GetPbType(this, BufferHelper.GetUShort(array2, Project.IsPb5 ? 24 : 28));
					pbFunctionDefinition.Name = BufferHelper.GetString(Project.IsUnicode, FunctionBuffer, BufferHelper.GetUInt(array2, 0L));
					if (pbFunctionDefinition.Name.StartsWith("+"))
					{
						pbFunctionDefinition.Name = pbFunctionDefinition.Name.Substring(1);
					}
					pbFunctionDefinition.GlobalIndex = BufferHelper.GetUShort(array2, Project.IsPb5 ? 16 : 20);
					pbFunctionDefinition.RefIndex = BufferHelper.GetUShort(array2, Project.IsPb5 ? 18 : 22);
					pbFunctionDefinition.EventCode = BufferHelper.GetUShort(array2, Project.IsPb5 ? 28 : 32);
					pbFunctionDefinition.Params = new PbFunctionParam[0];
					uint uInt = BufferHelper.GetUInt(array2, Project.IsPb5 ? 4 : 8);
					if (uInt != 65535)
					{
						byte b = array2[Project.IsPb5 ? 26 : 30];
						pbFunctionDefinition.Params = new PbFunctionParam[b];
						for (int m = 0; m < b; m++)
						{
							PbFunctionParam pbFunctionParam = new PbFunctionParam();
							pbFunctionDefinition.Params[m] = pbFunctionParam;
							byte[] buffer2 = BufferHelper.GetBuffer(ParamBuffer, (int)uInt + m * 12, 12);
							if ((buffer2[10] & 4) == 4)
							{
								pbFunctionParam.IsReadOnly = true;
							}
							else if ((buffer2[10] & 2) == 2)
							{
								pbFunctionParam.IsReference = true;
							}
							pbFunctionParam.Type = PbType.GetPbType(this, BufferHelper.GetUShort(buffer2, 8L));
							pbFunctionParam.Name = BufferHelper.GetString(Project.IsUnicode, FunctionBuffer, (int)BufferHelper.GetUInt(buffer2, 0L));
							pbFunctionParam.ArrayString = PbVariable.GetArrayString(BufferHelper.GetUInt(buffer2, 4L), FunctionBuffer);
						}
					}
					uint uInt2 = BufferHelper.GetUInt(array2, Project.IsPb5 ? 8 : 12);
					if (uInt2 != 65535)
					{
						uint uInt3 = BufferHelper.GetUInt(array2, Project.IsPb5 ? 12 : 16);
						pbFunctionDefinition.Library = BufferHelper.GetString(Project.IsUnicode, FunctionBuffer, (int)uInt3);
						pbFunctionDefinition.Alias = BufferHelper.GetString(Project.IsUnicode, FunctionBuffer, (int)uInt2);
					}
					if (Project.Version > 146)
					{
						ushort uShort2 = BufferHelper.GetUShort(array2, 44L);
						if (uShort2 != ushort.MaxValue)
						{
							pbFunctionDefinition.ThrowsType = PbType.GetPbType(this, BufferHelper.GetUShort(FunctionBuffer, uShort2));
						}
					}
					PrintString(pbFunctionDefinition.ToString());
				}
			}
		}

		private void ReadFunction(PbFunction pbFunction, byte[] index)
		{
			using (new IndentBlock(this, string.Format("Function :{0}", string.Join(" ", index.Select((byte o) => string.Format("{0:X4}", o))))))
			{
				pbFunction.Index = BufferHelper.GetUShort(index, 2L);
				ushort num = ReadUShort();
				ushort num2 = ReadUShort();
				PrintString(string.Format("{0:X4} {1:X4} {2:X4}", num, num2, ReadUShort()));
				pbFunction.PCodeBytes = ReadBuffer(num);
				pbFunction.DebugBytes = ReadBuffer(num2 * 4);
				using (new IndentBlock(this, "PCodes"))
				{
					foreach (string item in PCodeHelper.ParsePCode(pbFunction, false))
					{
						PrintString(item);
					}
				}
				pbFunction.Variables = ReadVariables();
				pbFunction.Buffer = ReadStructBuffer();
				using (new IndentBlock(this, "Stack"))
				{
					PrintBuffer(pbFunction.Buffer, 16);
				}
				using (new IndentBlock(this, string.Format("Variable {0}", pbFunction.Variables.Length)))
				{
					for (int i = 0; i < pbFunction.Variables.Length; i++)
					{
						pbFunction.Variables[i].Object = pbFunction.Object;
						PrintString(string.Format("{0:X4}: {1}", i, pbFunction.Variables[i].ToString(pbFunction.Buffer, _isDebug)));
					}
				}
			}
		}

		private ushort ReadUShort()
		{
			_position += 2L;
			return BufferHelper.GetUShort(_dataBuffer, _position - 2);
		}

		private uint ReadUInt()
		{
			_position += 4L;
			return BufferHelper.GetUInt(_dataBuffer, _position - 4);
		}

		private byte[] ReadBuffer(int size)
		{
			_position += size;
			return BufferHelper.GetBuffer(_dataBuffer, _position - size, size);
		}

		private byte[] ReadStructBuffer()
		{
			uint size = ReadUInt();
			uint size2 = ReadUInt();
			byte[] result = ReadBuffer((int)size);
			ReadBuffer((int)size2);
			return result;
		}

		private PbType[] ReadTypes(bool isSystemEntry)
		{
			ReadBuffer(6);
			byte[] buffer = ReadStructBuffer();
			int num = (int)ReadUShort() / 20;
			PbType[] array = new PbType[num];
			for (ushort num2 = 0; num2 < num; num2 = (ushort)(num2 + 1))
			{
				byte[] array2 = ReadBuffer(20);
				array[num2] = new PbType(this, num2, BufferHelper.GetString(Project.IsUnicode, buffer, BufferHelper.GetUInt(array2, 8L)), array2[16] == 64, isSystemEntry);
				PrintString(string.Format("{0:X4} {1} {2}", num2, BufferHelper.GetHexString(array2), array[num2].Name));
			}
			return array;
		}

		private PbVariable[] ReadVariables(bool delayParseType = false)
		{
			ReadBuffer(6);
			byte[] structBuffer = ReadStructBuffer();
			int num = (int)ReadUShort() / 20;
			PbVariable[] array = new PbVariable[num];
			for (ushort num2 = 0; num2 < num; num2 = (ushort)(num2 + 1))
			{
				byte[] buffer = ReadBuffer(20);
				array[num2] = new PbVariable(this, num2, buffer, structBuffer, delayParseType);
			}
			return array;
		}

		private PbReferencedFunction[] ReadReferencedFunctions()
		{
			ReadBuffer(6);
			byte[] buffer = ReadStructBuffer();
			int num = (int)ReadUShort() / 20;
			PbReferencedFunction[] array = new PbReferencedFunction[num];
			for (ushort num2 = 0; num2 < num; num2 = (ushort)(num2 + 1))
			{
				byte[] array2 = ReadBuffer(20);
				PbReferencedFunction pbReferencedFunction = (array[num2] = new PbReferencedFunction(num2, array2)
				{
					Name = BufferHelper.GetString(Project.IsUnicode, buffer, BufferHelper.GetUInt(array2, 8L)),
					GlobalIndex = BufferHelper.GetUShort(array2, 12L),
					IsGlobalFunction = (array2[16] == 2)
				});
			}
			return array;
		}

		public override string ToString()
		{
			return File.FileName + "/" + EntryName;
		}

		private void ParseSrj()
		{
			Source = Project.GetString(_entryData);
			string[] array = Source.Split(new char[2] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text in array)
			{
				if (text.StartsWith("PBD:"))
				{
					Project.OnNewLibrary(text.Substring(4).Split(',')[0], true);
				}
			}
		}

		private void ParseExe()
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			if (Project.IsUnicode)
			{
				int num = 0;
				int num2 = (_entryData[num + 1] << 8) + _entryData[num];
				num += 2;
				while (num2 > 0)
				{
					for (; _entryData[num] != 0 || _entryData[num + 1] != 0; num += 2)
					{
					}
					num += 2;
					num2--;
				}
				num2 = (_entryData[num + 1] << 8) + _entryData[num];
				num += 2;
				int num3 = num;
				while (num2 > 0)
				{
					for (; _entryData[num] != 0 || _entryData[num + 1] != 0; num += 2)
					{
					}
					list.Add(Encoding.Unicode.GetString(_entryData, num3, num - num3));
					num += 2;
					num3 = num;
					num2--;
				}
				num2 = (_entryData[num + 1] << 8) + _entryData[num];
				num += 2;
				num3 = num;
				while (num2 > 0)
				{
					for (; _entryData[num] != 0 || _entryData[num + 1] != 0; num += 2)
					{
					}
					list2.Add(Encoding.Unicode.GetString(_entryData, num3, num - num3));
					num += 2;
					num3 = num;
					num2--;
				}
			}
			else
			{
				int num4;
				int j;
				if (Project.IsPb5)
				{
					j = 0;
					num4 = 1;
				}
				else
				{
					j = 1;
					num4 = _entryData[0];
				}
				while (num4 > 0)
				{
					for (; _entryData[j] != 0; j++)
					{
					}
					j++;
					num4--;
				}
				num4 = (_entryData[j + 1] << 8) + _entryData[j];
				j += 2;
				int num5 = j;
				while (num4 > 0)
				{
					for (; _entryData[j] != 0; j++)
					{
					}
					list.Add(Encoding.Default.GetString(_entryData, num5, j - num5));
					j++;
					num5 = j;
					num4--;
				}
				num4 = (_entryData[j + 1] << 8) + _entryData[j];
				j += 2;
				num5 = j;
				while (num4 > 0)
				{
					for (; _entryData[j] != 0; j++)
					{
					}
					list2.Add(Encoding.Default.GetString(_entryData, num5, j - num5));
					j++;
					num5 = j;
					num4--;
				}
			}
			foreach (string item in list)
			{
				Project.OnNewLibrary(item, false);
			}
			Source = string.Format("Libraries {0:X4}:\r\n\t", list.Count) + string.Join("\r\n\t", list.Select((string o, int i) => string.Format("{0:X4}:\t{1}", i, o))) + string.Format("\r\nEntries {0:X4}:\r\n\t", list2.Count) + string.Join("\r\n\t", list2.Select((string o, int i) => string.Format("{0:X4}:\t{1}", i, o)));
		}

		public void OnNewType(PbType pbType)
		{
			Types[pbType.Index] = pbType;
		}

		public void ParseInherit()
		{
			foreach (PbObject value in Objects.Values)
			{
				value.ParseInherit();
			}
		}
	}
}
