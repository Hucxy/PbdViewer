using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PbdViewer.Uitils.PbClass;

namespace PbdViewer.Uitils.PCode
{
	internal abstract class PCodeParserBase
	{
		private class StackObject
		{
			public PbType Type { get; set; }

			public string Str { get; set; }

			public string Opreator { get; set; }

			public StackObject(string str, PbType type = null)
			{
				Type = type;
				Str = str;
			}

			public override string ToString()
			{
				return Str;
			}
		}

		private readonly Stack<StackObject> _stackObjects = new Stack<StackObject>();

		private CodeLine _codeLine;

		protected readonly PbFunction PbFunction;

		protected abstract byte[] PCodeLenArray { get; }

		public void ParsePCode(CodeLine codeLine)
		{
			_codeLine = codeLine;
			_codeLine.SCode = "";
			if (!OnParsePcode(codeLine.PCodeOp, codeLine))
			{
				codeLine.SCode = string.Format("-------{0:X4}", codeLine.PCodeOp);
			}
		}

		public byte GetPCodeLen(ushort pcode)
		{
			return OnGetPCodeLen(pcode);
		}

		protected PCodeParserBase(PbFunction pbFunction)
		{
			PbFunction = pbFunction;
		}

		protected virtual byte OnGetPCodeLen(ushort pcode)
		{
			if (pcode < PCodeLenArray.Length)
			{
				return PCodeLenArray[pcode];
			}
			return byte.MaxValue;
		}

		protected abstract bool OnParsePcode(int pCodeOp, CodeLine codeLine);

		protected void BeginAssignLocalVariable(ushort index)
		{
			PushVariable(PbFunction.Variables[index]);
		}

		protected void BeginAssignSharedVariable(ushort index)
		{
			PushVariable(PbFunction.Entry.Variables[index]);
		}

		protected void BeginAssignGlobalVariable(ushort index)
		{
			PushVariable(PbFunction.Variables.First((PbVariable o) => o.GlobalIndex == index));
		}

		protected void BeginAssignInstanceVariable()
		{
			StackObject stackObject = _stackObjects.Pop();
			StackObject stackObject2 = _stackObjects.Pop();
			if (stackObject2.Str == "entryobject")
			{
				_stackObjects.Push(new StackObject(string.Format("{0}", stackObject.Str), stackObject.Type));
			}
			else
			{
				_stackObjects.Push(new StackObject(string.Format("{0}.{1}", stackObject2.Str, stackObject.Str), stackObject.Type));
			}
		}

		protected void EndAssign(bool isArray = false)
		{
			StackObject stackObject = _stackObjects.Pop();
			StackObject stackObject2 = _stackObjects.Pop();
			if (isArray)
			{
				_codeLine.SCode = string.Format("{0}[] = {1}", stackObject2.Str, stackObject.Str);
			}
			else
			{
				_codeLine.SCode = string.Format("{0} = {1}", stackObject2.Str, stackObject.Str);
			}
		}

		protected void EndAssign(string operater)
		{
			StackObject stackObject = _stackObjects.Pop();
			StackObject stackObject2 = _stackObjects.Pop();
			_codeLine.SCode = string.Format("{0} {1}= {2}", stackObject2.Str, operater, stackObject.Str);
		}

		protected void EndAssign2(string operater)
		{
			StackObject stackObject = _stackObjects.Pop();
			_codeLine.SCode = string.Format("{0} {1}", stackObject.Str, operater);
		}

		protected void ResetAssign(ushort count)
		{
			StackObject item = _stackObjects.Pop();
			StackObject item2 = _stackObjects.Peek();
			_stackObjects.Push(item2);
			_stackObjects.Push(item);
		}

		protected void PushLocalVariable(ushort index)
		{
			PushVariable(PbFunction.Variables[index]);
		}

		protected void PushSharedVariable(ushort index)
		{
			PushVariable(PbFunction.Entry.Variables[index]);
		}

		protected void PushGlobalSharedVariable(ushort index)
		{
			PushVariable(PbFunction.Entry.Variables.FirstOrDefault((PbVariable o) => o.GlobalIndex == index));
		}

		protected void PushGlobalVariable(ushort index)
		{
			PushVariable(PbFunction.Variables.First((PbVariable o) => o.GlobalIndex == index));
		}

		protected void PushInstanceVariable(ushort unkown = 0)
		{
			StackObject stackObject = _stackObjects.Pop();
			StackObject stackObject2 = _stackObjects.Pop();
			if (stackObject2.Str == "entryobject")
			{
				_stackObjects.Push(new StackObject(string.Format("{0}", stackObject.Str), stackObject.Type));
			}
			else
			{
				_stackObjects.Push(new StackObject(string.Format("{0}.{1}", stackObject2.Str, stackObject.Str), stackObject.Type));
			}
		}

		protected void PushInstanceVariableName(uint offset)
		{
			if (offset >= 1 && offset <= 7)
			{
				_stackObjects.Push(new StackObject("entryobject", PbFunction.Entry.EntryObject.Type));
				return;
			}
			uint uInt = BufferHelper.GetUInt(PbFunction.Buffer, offset);
			PbType type = _stackObjects.Peek().Type;
			object obj;
			if (type == null)
			{
				obj = null;
			}
			else
			{
				PbObject @object = type.GetObject(PbFunction.Entry);
				obj = ((@object != null) ? @object.AllVariables[BufferHelper.GetUShort(PbFunction.Buffer, offset + 4)] : null);
			}
			PbVariable pbVariable = (PbVariable)obj;
			string text;
			if (pbVariable == null)
			{
				text = (((uInt & 0xFFFF) == 65535) ? string.Format("{0:X4}", BufferHelper.GetUShort(PbFunction.Buffer, offset + 4)) : BufferHelper.GetString(PbFunction.Project.IsUnicode, PbFunction.Buffer, uInt));
			}
			else if ((uInt & 0xFFFF) != 65535)
			{
				text = BufferHelper.GetString(PbFunction.Project.IsUnicode, PbFunction.Buffer, uInt);
				if (!string.Equals(pbVariable.Name, text, StringComparison.CurrentCultureIgnoreCase))
				{
					throw new Exception(string.Format("PushInstanceVariableName \"{0}\" != \"{1}\"", pbVariable.Name, text));
				}
			}
			else
			{
				text = pbVariable.Name;
			}
			_stackObjects.Push(new StackObject(text, (pbVariable != null) ? pbVariable.Type : null));
		}

		protected void PushConstant(string constant)
		{
			_stackObjects.Push(new StackObject(constant));
		}

		protected void PushThis()
		{
			_stackObjects.Push(new StackObject("this", PbFunction.Object.Type));
		}

		protected void PushParent()
		{
			Stack<StackObject> stackObjects = _stackObjects;
			PbObject parentObject = PbFunction.Object.ParentObject;
			stackObjects.Push(new StackObject("parent", (parentObject != null) ? parentObject.Type : null));
		}

		protected void PushEnum(ushort enumIndex, ushort itemIndex)
		{
			_stackObjects.Push(new StackObject(PbFunction.Project.Enums[enumIndex].Items[itemIndex], PbType.GetPbType(PbFunction.Entry, enumIndex)));
		}

		protected void OperateStack(string op)
		{
			int operatorLevel = GetOperatorLevel(op);
			StackObject stackObject = _stackObjects.Pop();
			int operatorLevel2 = GetOperatorLevel(stackObject.Opreator);
			if (operatorLevel2 > operatorLevel || operatorLevel2 == operatorLevel)
			{
				stackObject.Str = string.Format("({0})", stackObject.Str);
			}
			StackObject stackObject2 = _stackObjects.Pop();
			if (GetOperatorLevel(stackObject2.Opreator) > operatorLevel)
			{
				stackObject2.Str = string.Format("({0})", stackObject2.Str);
			}
			string str = stackObject2.Str + " " + op + " " + stackObject.Str;
			_stackObjects.Push(new StackObject(str)
			{
				Opreator = op
			});
		}

		protected void OperateStackSingle(string op)
		{
			StackObject stackObject = _stackObjects.Pop();
			if (GetOperatorLevel(stackObject.Opreator) > 0)
			{
				stackObject.Str = string.Format("({0})", stackObject.Str);
			}
			string str = op + " " + stackObject.Str;
			_stackObjects.Push(new StackObject(str)
			{
				Opreator = "$" + op
			});
		}

		protected void Return(ushort p1 = 0)
		{
			string sCode = ((p1 == 1) ? string.Format("return {0}", _stackObjects.Pop().Str) : "return");
			_codeLine.SCode = sCode;
		}

		protected void Halt(ushort force)
		{
			string text = "halt";
			if (force == 0)
			{
				text += " close";
			}
			_codeLine.SCode = text;
		}

		protected void Jump(ushort pos, JmpType jmpType)
		{
			_codeLine.JmpType = jmpType;
			_codeLine.JmpPositon = pos;
			switch (jmpType)
			{
			case JmpType.Jmp:
				_codeLine.SCode = string.Format("goto {0:X4}", pos);
				break;
			case JmpType.JmpIfTrue:
				_codeLine.Condition = _stackObjects.Pop().Str;
				_codeLine.SCode = string.Format("if {0} then goto {1:X4}", _codeLine.Condition, pos);
				break;
			case JmpType.JmpIfFalse:
				_codeLine.Condition = _stackObjects.Pop().Str;
				_codeLine.SCode = string.Format("if {0} then not goto {1:X4}", _codeLine.Condition, pos);
				break;
			}
		}

		protected void Try(ushort catchpos, ushort endpos)
		{
			_codeLine.SCode = "try ";
		}

		protected void EndTry()
		{
			_codeLine.SCode = "end try ";
		}

		protected void Catch()
		{
			StackObject stackObject = _stackObjects.Pop();
			_stackObjects.Push(new StackObject(string.Format("catch ({0} {1})", stackObject.Type.Name, stackObject.Str)));
		}

		protected void Throw()
		{
			StackObject arg = _stackObjects.Pop();
			_codeLine.SCode = string.Format("throw {0}", arg);
		}

		protected void EnterFinally(ushort finallypos)
		{
			_codeLine.SCode = "enter finally ";
			_codeLine.JmpPositon = finallypos;
		}

		protected void LeaveFinally()
		{
		}

		protected void CreateObject(uint offset)
		{
			PbType typeName = GetTypeName(offset);
			_stackObjects.Push(new StackObject(string.Format("create {0}", typeName.Name), typeName));
		}

		protected void CreateObjectUsingName(uint index)
		{
			_stackObjects.Push(new StackObject(string.Format("create using {0}", _stackObjects.Pop()), PbType.GetPbType(PbFunction.Entry, 8)));
		}

		protected void DestroyObject()
		{
			StackObject stackObject = _stackObjects.Pop();
			_codeLine.SCode = string.Format("destroy({0})", stackObject.Str);
		}

		protected void PopFunction()
		{
			_codeLine.SCode = string.Format("{0}", _stackObjects.Pop().Str);
		}

		protected void PushGlobalFunctionName(ushort objIndex, ushort functionIndex)
		{
			string str = null;
			if ((objIndex & 0x8000) == 32768)
			{
				str = PbFunction.Object.ReferencedFunctions[functionIndex].Name;
			}
			else if ((objIndex & 0x4000) == 16384)
			{
				PbEntry systemEntry = PbFunction.Project.SystemEntry;
				object obj;
				if (systemEntry == null)
				{
					obj = null;
				}
				else
				{
					PbObject pbObject = systemEntry.Objects[objIndex];
					obj = ((pbObject != null) ? pbObject.FunctionDefinitions[functionIndex] : null);
				}
				PbFunctionDefinition pbFunctionDefinition = (PbFunctionDefinition)obj;
				str = ((pbFunctionDefinition == null) ? string.Format("({0:X4}{1:X4})", objIndex, functionIndex) : pbFunctionDefinition.Name);
			}
			_stackObjects.Push(new StackObject(str));
		}

		protected void CallGlobalFunction(uint count, uint type)
		{
			string text = _stackObjects.Pop().Str;
			IEnumerable<StackObject> source = PopStack(count);
			if ((type & 1) == 1)
			{
				text = "post " + text;
			}
			if ((type & 2) == 2)
			{
				text = "dynamic " + text;
			}
			if ((type & 4) == 4)
			{
				text = "event " + text;
			}
			string str = string.Format("{0}({1})", text, string.Join(",", source.Select((StackObject o) => o.Str)));
			_stackObjects.Push(new StackObject(str));
		}

		protected void CallSuper(uint functionIndex, ushort paramcount, ushort objType, uint nameoffset)
		{
			object[] array = new object[paramcount];
			for (int i = 0; i < paramcount; i++)
			{
				array[i] = _stackObjects.Pop();
			}
			string str = "call super::" + BufferHelper.GetString(PbFunction.Project.IsUnicode, PbFunction.Buffer, nameoffset);
			_stackObjects.Push(new StackObject(str));
		}

		protected void CallFunction(uint offset, uint count, uint type)
		{
			ushort uShort = BufferHelper.GetUShort(PbFunction.Buffer, offset);
			PbType pbType = PbType.GetPbType(PbFunction.Entry, BufferHelper.GetUShort(PbFunction.Buffer, offset + 2));
			uint uInt = BufferHelper.GetUInt(PbFunction.Buffer, offset + 4);
			if ((uInt & 0xFFFF) == 65535)
			{
				throw new Exception("CallFunction funnameoffset==0xFFFF");
			}
			IEnumerable<StackObject> source = PopStack(count);
			StackObject stackObject = _stackObjects.Pop();
			string str = stackObject.Str;
			string text = BufferHelper.GetString(PbFunction.Project.IsUnicode, PbFunction.Buffer, uInt);
			PbFunctionDefinition pbFunctionDefinition = null;
			if (uShort != ushort.MaxValue && stackObject.Type != null && stackObject.Type.Name != "any")
			{
				PbObject @object = stackObject.Type.GetObject(PbFunction.Entry);
				pbFunctionDefinition = ((@object != null) ? @object.AllFunctionDefinitions[uShort] : null);
				if (pbFunctionDefinition != null)
				{
					object obj;
					if (pbFunctionDefinition == null)
					{
						obj = null;
					}
					else
					{
						string name = pbFunctionDefinition.Name;
						obj = ((name != null) ? name.ToLower() : null);
					}
					if ((string)obj != ((text != null) ? text.ToLower() : null))
					{
						pbFunctionDefinition = null;
					}
				}
			}
			if ((type & 1) == 1)
			{
				text = "post " + text;
			}
			if ((type & 2) == 2)
			{
				text = "dynamic " + text;
			}
			if ((type & 4) == 4)
			{
				text = "event " + text;
			}
			str = ((!(stackObject.Str == "this") || string.IsNullOrEmpty(pbType.Name)) ? (str + ".") : "super::");
			string str2 = string.Format("{0}{1}({2})", str, text, string.Join(",", source.Select((StackObject o) => o.Str)));
			Stack<StackObject> stackObjects = _stackObjects;
			PbType type2 = stackObject.Type;
			stackObjects.Push(new StackObject(str2, (((type2 != null) ? type2.Name : null) == "any") ? stackObject.Type : ((pbFunctionDefinition != null) ? pbFunctionDefinition.ReturnType : null)));
		}

		protected void CallBuiltinFunction(string function, int paramcount = 1)
		{
			IEnumerable<StackObject> source = PopStack(paramcount);
			string str = string.Format("{0}({1})", function, string.Join(",", source.Select((StackObject o) => o.Str)));
			_stackObjects.Push(new StackObject(str));
		}

		protected void CreateArray(uint arraylen)
		{
			IEnumerable<StackObject> source = PopStack(arraylen);
			string str = string.Format("{{{0}}}", string.Join(",", source.Select((StackObject o) => o.Str)));
			_stackObjects.Push(new StackObject(str));
		}

		protected void Index()
		{
			StackObject stackObject = _stackObjects.Pop();
			StackObject stackObject2 = _stackObjects.Pop();
			string str = string.Format("{0}[{1}]", stackObject2.Str, stackObject.Str);
			_stackObjects.Push(new StackObject(str, stackObject2.Type));
		}

		protected void Index2(ushort p1, ushort p2)
		{
			StackObject stackObject = _stackObjects.Pop();
			StackObject stackObject2 = _stackObjects.Pop();
			string str = string.Format("{0}[{1}]", stackObject2.Str, stackObject.Str);
			_stackObjects.Push(new StackObject(str, stackObject2.Type));
		}

		protected void Index3(ushort p1, ushort p2, ushort p3)
		{
			PopStack(2L);
			StackObject stackObject = _stackObjects.Pop();
			StackObject stackObject2 = _stackObjects.Pop();
			string str = string.Format("{0}[{1}]", stackObject2.Str, stackObject.Str);
			_stackObjects.Push(new StackObject(str, stackObject2.Type));
		}

		protected void Cast(ushort pos = 0)
		{
		}

		protected void SqlOperateTransaction(string function)
		{
			StackObject arg = _stackObjects.Pop();
			_codeLine.SCode = string.Format("{0} using {1};", function, arg);
		}

		protected void SqlOpen(ushort paramCount)
		{
			IEnumerable<StackObject> source = PopStack(paramCount);
			StackObject stackObject = _stackObjects.Pop();
			StackObject cusor = _stackObjects.Pop();
			PbVariable pbVariable = PbFunction.Variables.FirstOrDefault((PbVariable o) => o.Name == cusor.Str);
			if (pbVariable != null)
			{
				pbVariable.SetCursorParams(source.Select((StackObject o) => o.Str), stackObject.Str);
			}
			_codeLine.SCode = string.Format("open {0};", cusor);
		}

		protected void SqlOpenDynamic(uint cursorOffset, ushort paramCount)
		{
			StackObject stackObject = _stackObjects.Pop();
			StackObject cusor = _stackObjects.Pop();
			IEnumerable<StackObject> source = PopStack(paramCount);
			PbVariable pbVariable = PbFunction.Variables.FirstOrDefault((PbVariable o) => o.Name == cusor.Str);
			if (pbVariable != null)
			{
				pbVariable.SetDynamicCursorParams(stackObject.Str);
			}
			string arg = ((paramCount > 0) ? string.Format("using {0}", string.Join(",", source.Select((StackObject o) => string.Format(":{0}", o)))) : "");
			_codeLine.SCode = string.Format("open dynamic {0} {1};", cusor, arg);
		}

		protected void SqlExecute(ushort paramcount)
		{
			IEnumerable<StackObject> source = PopStack(paramcount);
			StackObject stackObject = _stackObjects.Pop();
			StackObject procedure = _stackObjects.Pop();
			PbVariable pbVariable = PbFunction.Variables.FirstOrDefault((PbVariable o) => o.Name == procedure.Str);
			if (pbVariable != null)
			{
				pbVariable.SetProcdureParams(source.Select((StackObject o) => o.Str), stackObject.Str);
			}
			_codeLine.SCode = string.Format("execute {0};", procedure);
		}

		protected void SqlExecuteDynamic(uint procedureOffset, ushort paramCount)
		{
			StackObject stackObject = _stackObjects.Pop();
			StackObject cusor = _stackObjects.Pop();
			IEnumerable<StackObject> source = PopStack(paramCount);
			PbVariable pbVariable = PbFunction.Variables.FirstOrDefault((PbVariable o) => o.Name == cusor.Str);
			if (pbVariable != null)
			{
				pbVariable.SetDynamictProcdureParams(stackObject.Str);
			}
			string arg = ((paramCount > 0) ? string.Format("using {0}", string.Join(",", source.Select((StackObject o) => string.Format(":{0}", o)))) : "");
			_codeLine.SCode = string.Format("execute dynamic {0} {1};", cusor, arg);
		}

		protected void SqlFetch(ushort paramcount)
		{
			_stackObjects.Pop();
			StackObject stackObject = _stackObjects.Pop();
			IEnumerable<StackObject> source = PopStack(paramcount);
			_codeLine.SCode = string.Format("fetch {0} into {1};", stackObject.Str, string.Join(",", source.Select((StackObject o) => string.Format(":{0}", o.Str))));
		}

		protected void SqlClose()
		{
			_stackObjects.Pop();
			StackObject stackObject = _stackObjects.Pop();
			_codeLine.SCode = string.Format("close {0};", stackObject.Str);
		}

		protected void SqlPrepareSqlsa()
		{
			StackObject arg = _stackObjects.Pop();
			string text = _stackObjects.Pop().Str;
			if (!text.StartsWith("\""))
			{
				text = ":" + text;
			}
			StackObject arg2 = _stackObjects.Pop();
			_codeLine.SCode = string.Format("prepare {0} from {1} using {2};", arg2, text, arg);
		}

		protected void SqlExecuteSqlsa(ushort paramcount)
		{
			IEnumerable<StackObject> source = PopStack(paramcount);
			StackObject arg = _stackObjects.Pop();
			_codeLine.SCode = string.Format("execute {0} using {1};", arg, string.Join(",", source.Select((StackObject o) => string.Format(":{0}", o))));
		}

		protected void SqlExecuteImmediate()
		{
			StackObject arg = _stackObjects.Pop();
			string text = _stackObjects.Pop().Str;
			if (!text.StartsWith("\""))
			{
				text = ":" + text;
			}
			_codeLine.SCode = string.Format("execute immediate {0} using {1};", text, arg);
		}

		protected void SqlDescribe()
		{
			StackObject arg = _stackObjects.Pop();
			StackObject arg2 = _stackObjects.Pop();
			_codeLine.SCode = string.Format("describe {0} into {1};", arg2, arg);
		}

		protected void SqlOpenDynamicDescriptor(uint cursorOffset)
		{
			StackObject stackObject = _stackObjects.Pop();
			StackObject cusor = _stackObjects.Pop();
			StackObject arg = _stackObjects.Pop();
			PbVariable pbVariable = PbFunction.Variables.FirstOrDefault((PbVariable o) => o.Name == cusor.Str);
			if (pbVariable != null)
			{
				pbVariable.SetDynamicCursorParams(stackObject.Str);
			}
			_codeLine.SCode = string.Format("open dynamic {0} using descriptor {1};", cusor, arg);
		}

		protected void SqlExecuteDynamicDescriptor(uint procedureOffset)
		{
			StackObject stackObject = _stackObjects.Pop();
			StackObject cusor = _stackObjects.Pop();
			StackObject arg = _stackObjects.Pop();
			PbVariable pbVariable = PbFunction.Variables.FirstOrDefault((PbVariable o) => o.Name == cusor.Str);
			if (pbVariable != null)
			{
				pbVariable.SetDynamicCursorParams(stackObject.Str);
			}
			_codeLine.SCode = string.Format("execute dynamic {0} using descriptor {1};", cusor, arg);
		}

		protected void SqlFetchDynamicDescriptor()
		{
			_stackObjects.Pop();
			StackObject arg = _stackObjects.Pop();
			StackObject arg2 = _stackObjects.Pop();
			_codeLine.SCode = string.Format("fetch {0} using descriptor {1};", arg, arg2);
		}

		protected void SqlDirectInsertUpdateDelete(uint cursorOffset, ushort paramcount)
		{
			StackObject arg = _stackObjects.Pop();
			IEnumerable<StackObject> source = PopStack(paramcount);
			string cursor = BufferHelper.GetCursor(PbFunction.Project.IsUnicode, PbFunction.Entry.VariableBuffer, cursorOffset, source.Select((StackObject o) => o.Str));
			_codeLine.SCode = string.Format("{0} using {1};", cursor, arg);
		}

		protected void SqlDirectSelect(uint cursorOffset, ushort paramcount1, ushort paramcount2)
		{
			StackObject arg = _stackObjects.Pop();
			IEnumerable<StackObject> source = PopStack(paramcount1);
			IEnumerable<StackObject> source2 = PopStack(paramcount2);
			string cursor = BufferHelper.GetCursor(PbFunction.Project.IsUnicode, PbFunction.Entry.VariableBuffer, cursorOffset, source.Select((StackObject o) => o.Str));
			cursor = new Regex(" from ", RegexOptions.IgnoreCase).Replace(cursor, string.Format(" into {0} from ", string.Join(",", source2.Select((StackObject o) => string.Format(":{0}", o)))), 1);
			_codeLine.SCode = string.Format("{0} using {1};", cursor, arg);
		}

		private PbType GetTypeName(uint offset)
		{
			uint uInt = BufferHelper.GetUInt(PbFunction.Buffer, offset);
			ushort uShort = BufferHelper.GetUShort(PbFunction.Buffer, offset + 4);
			ushort uShort2 = BufferHelper.GetUShort(PbFunction.Buffer, offset + 6);
			if (uShort2 != ushort.MaxValue)
			{
			}
			PbType pbType = PbType.GetPbType(PbFunction.Entry, uShort);
			string @string = BufferHelper.GetString(PbFunction.Project.IsUnicode, PbFunction.Buffer, uInt);
			if (pbType.Name != @string)
			{
				throw new Exception(string.Format("GetTypeName \"{0}\" != \"{1}\"", pbType.Name, @string));
			}
			return pbType;
		}

		private static int GetOperatorLevel(string @operator)
		{
			switch (@operator)
			{
			case "+":
			case "-":
				return 5;
			case "*":
			case "/":
				return 4;
			case "^":
				return 3;
			case "and":
			case "or":
				return 2;
			case "=":
			case "<>":
			case ">":
			case "<":
			case ">=":
			case "<=":
				return 1;
			case "$not":
			case "$-":
				return 6;
			default:
				return 0;
			}
		}

		private void PushVariable(PbVariable variable)
		{
			_stackObjects.Push(new StackObject(variable.Name, variable.Type));
		}

		private void Check()
		{
			byte[] pCodeLenArray = new PCodeParser105(PbFunction).PCodeLenArray;
			byte[] pCodeLenArray2 = new PCodeParser110(PbFunction).PCodeLenArray;
			int num = Math.Min(pCodeLenArray.Length, pCodeLenArray2.Length);
			for (int i = 0; i < num && pCodeLenArray[i] == pCodeLenArray2[i]; i++)
			{
			}
			for (int j = 0; j < num && pCodeLenArray[pCodeLenArray.Length - 1 - j] == pCodeLenArray2[pCodeLenArray2.Length - 1 - j]; j++)
			{
			}
		}

		private string GetArray(string filename)
		{
			byte[] array = new byte[4096];
			List<byte> list = new List<byte>();
			FileStream fileStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			int num = 0;
			bool flag = false;
			int num2 = fileStream.Read(array, num, 4096);
			byte[] array2 = new byte[13]
			{
				193, 224, 2, 139, 255, 255, 255, 255, 255, 141,
				84, 255, 2
			};
			while (num2 >= array2.Length)
			{
				for (int i = 0; i < num2 - array2.Length; i++)
				{
					bool flag2 = true;
					for (int j = 0; j < array2.Length; j++)
					{
						if (array2[j] != byte.MaxValue && array[i + j] != array2[j])
						{
							flag2 = false;
							break;
						}
					}
					if (flag2)
					{
						flag = true;
						num += i + 5;
						break;
					}
				}
				if (flag)
				{
					break;
				}
				int num3 = 1 - array2.Length;
				num += num2 + num3;
				fileStream.Seek(num3, SeekOrigin.Current);
				num2 = fileStream.Read(array, 0, 4096);
			}
			if (!flag)
			{
				return "";
			}
			fileStream.Seek(num, SeekOrigin.Begin);
			fileStream.Read(array, 0, 4);
			uint uInt = BufferHelper.GetUInt(array, 0L);
			uInt = new PEHelper(fileStream).GetOffset(uInt) - 4;
			fileStream.Seek(uInt, SeekOrigin.Begin);
			while (true)
			{
				fileStream.Read(array, 0, 12);
				if (BufferHelper.GetUInt(array, 4L) > 255)
				{
					break;
				}
				list.Add(array[4]);
			}
			fileStream.Close();
			string text = string.Empty;
			int num4 = list.Count / 16;
			for (int k = 0; k < num4; k++)
			{
				for (int l = 0; l < 16; l++)
				{
					text = text + list[k * 16 + l] + ",";
				}
				if (k % 8 == 0)
				{
					text += string.Format("\t\t//0x{0:X2}", k);
				}
				text += "\r\n";
			}
			for (int m = 0; m < list.Count - num4 * 16; m++)
			{
				text = text + list[num4 * 16 + m] + ",";
			}
			return text;
		}

		private IEnumerable<StackObject> PopStack(long count)
		{
			StackObject[] array = new StackObject[count];
			for (int i = 0; i < count; i++)
			{
				array[count - 1 - i] = _stackObjects.Pop();
			}
			return array;
		}
	}
}
