using System;
using System.Collections.Generic;
using System.Linq;
using PbdViewer.Uitils.PbClass;
using PbdViewer.Uitils.PCode;

namespace PbdViewer.Uitils
{
	public static class PCodeHelper
	{
		public static readonly Dictionary<ushort, int> Count = new Dictionary<ushort, int>();

		public static readonly Dictionary<ushort, int> Goodcount = new Dictionary<ushort, int>();

		public static readonly Dictionary<ushort, HashSet<ushort>> UsedPCodeList = new Dictionary<ushort, HashSet<ushort>>();

		public static readonly Dictionary<ushort, HashSet<ushort>> UnParsedPCodeList = new Dictionary<ushort, HashSet<ushort>>();

		public static IEnumerable<string> ParsePCode(PbFunction pbFunction, bool depth)
		{
			if (pbFunction.Project.IsDebug && !depth)
			{
				return PrivaeParsePCode(pbFunction, false);
			}
			List<string> list = new List<string>();
			foreach (string item in PrivaeParsePCode(pbFunction, depth))
			{
				string[] array = item.Split(new char[2] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string text in array)
				{
					string text2 = text;
					if (!pbFunction.Project.IsDebug)
					{
						text2 = text.Substring(48);
						if (string.IsNullOrWhiteSpace(text2))
						{
							continue;
						}
					}
					list.Add(text2);
				}
			}
			return list;
		}

		private static IEnumerable<string> PrivaeParsePCode(PbFunction pbFunction, bool depth)
		{
			ushort version = pbFunction.Project.Version;
			Init(version);
			Dictionary<ushort, CodeLine> dictionary = new Dictionary<ushort, CodeLine>();
			if (pbFunction.PCodeBytes.Length == 0)
			{
				return dictionary.Select((KeyValuePair<ushort, CodeLine> o) => o.ToString());
			}
			PCodeParserBase pCodeParse = GetPCodeParse(pbFunction, pbFunction.Project.Version);
			Dictionary<ushort, ushort> dictionary2 = new Dictionary<ushort, ushort>();
			for (int i = 0; i < pbFunction.DebugBytes.Length / 4; i++)
			{
				byte[] buffer = BufferHelper.GetBuffer(pbFunction.DebugBytes, i * 4, 4);
				dictionary2[BufferHelper.GetUShort(buffer, 2L)] = BufferHelper.GetUShort(buffer, 0L);
			}
			bool flag = false;
			ushort num = 0;
			CodeLine codeLine = null;
			while (num < pbFunction.PCodeBytes.Length)
			{
				ushort uShort = BufferHelper.GetUShort(pbFunction.PCodeBytes, num);
				byte pCodeLen = pCodeParse.GetPCodeLen(uShort);
				if (pCodeLen == byte.MaxValue)
				{
					flag = true;
					break;
				}
				CodeLine codeLine2 = new CodeLine
				{
					PCodePositon = num,
					DebugLine = (dictionary2.ContainsKey(num) ? new ushort?(dictionary2[num]) : null),
					PCodeOp = uShort,
					PCodeParam = BufferHelper.GetBuffer(pbFunction.PCodeBytes, num + 2, pCodeLen * 2)
				};
				if (codeLine != null)
				{
					codeLine.NextCodeLine = codeLine2;
					codeLine2.PreCodeLine = codeLine;
				}
				codeLine = codeLine2;
				if (depth)
				{
					try
					{
						if (!flag)
						{
							pCodeParse.ParsePCode(codeLine2);
							if (codeLine2.SCode.StartsWith("--") && !UnParsedPCodeList[version].Contains(codeLine2.PCodeOp))
							{
								UnParsedPCodeList[version].Add(codeLine2.PCodeOp);
							}
						}
					}
					catch (Exception)
					{
						flag = true;
					}
				}
				UsedPCodeList[version].Add(codeLine2.PCodeOp);
				dictionary.Add(codeLine2.PCodePositon, codeLine2);
				num = (ushort)(num + (ushort)(2 + pCodeLen * 2));
			}
			if (depth)
			{
				try
				{
					ParseJmp(pbFunction, dictionary);
				}
				catch (Exception)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				Goodcount[version]++;
			}
			Count[version]++;
			return dictionary.Values.Select((CodeLine o) => o.ToString());
		}

		private static void ParseJmp(PbFunction pbFunction, Dictionary<ushort, CodeLine> list)
		{
			List<CodeArea> areas = new List<CodeArea>();
			if (pbFunction.Project.Version >= 283)
			{
				ParseReturn(list);
			}
			ParseTryCatchFinally(list, areas);
			ParseChoose(list, areas);
			ParseForNext(list, areas);
			ParseDoLoop(list, areas);
			ParseExitContinue(list, areas);
			ParseIfElse(list);
			ParseGoto(list);
			ParseEventReturn(list);
			ParseIndent(pbFunction, list);
		}

		private static void ParseReturn(Dictionary<ushort, CodeLine> list)
		{
			foreach (CodeLine value in list.Values)
			{
				if (value.SCode.StartsWith("return ") && value.NextCodeLine.JmpType == JmpType.Jmp && list[value.NextCodeLine.JmpPositon].SCode == "return")
				{
					value.NextCodeLine.SCode = "";
					value.NextCodeLine.JmpType = JmpType.None;
				}
			}
		}

		private static void ParseTryCatchFinally(Dictionary<ushort, CodeLine> list, List<CodeArea> areas)
		{
			foreach (CodeLine value in list.Values)
			{
				if (string.IsNullOrEmpty(value.SCode))
				{
					continue;
				}
				if (value.JmpType == JmpType.JmpIfFalse && value.Condition.StartsWith("catch ("))
				{
					value.SCode = value.Condition;
					value.JmpType = JmpType.None;
					if (!list[value.JmpPositon].SCode.StartsWith("end try ") && !list[value.JmpPositon].SCode.StartsWith("enter finally ") && list[value.JmpPositon].PreCodeLine.JmpType == JmpType.Jmp && (list[list[value.JmpPositon].PreCodeLine.JmpPositon].SCode.StartsWith("end try ") || list[list[value.JmpPositon].PreCodeLine.JmpPositon].SCode.StartsWith("enter finally ")))
					{
						list[value.JmpPositon].PreCodeLine.SCode = "";
						list[value.JmpPositon].PreCodeLine.JmpType = JmpType.None;
					}
				}
				if (value.JmpType == JmpType.Jmp && (list[value.JmpPositon].SCode.StartsWith("end try ") || list[value.JmpPositon].SCode.StartsWith("enter finally ")))
				{
					value.SCode = "";
					value.JmpType = JmpType.None;
				}
				if (value.SCode == "enter finally ")
				{
					value.SCode = "";
					list[value.JmpPositon].LabelSCode.Add("finally ");
				}
			}
		}

		private static void ParseChoose(Dictionary<ushort, CodeLine> list, List<CodeArea> areas)
		{
			Dictionary<string, CodeArea> dictionary = new Dictionary<string, CodeArea>();
			foreach (CodeLine value in list.Values)
			{
				if (string.IsNullOrEmpty(value.SCode))
				{
					continue;
				}
				if (value.SCode.StartsWith("\u0001case"))
				{
					string text = value.SCode.Substring(0, value.SCode.IndexOf('=')).Trim();
					value.SCode = value.SCode.Replace(text + " = ", "choose case ");
					dictionary[text] = new CodeArea
					{
						Type = "choose",
						Start = value.PCodePositon
					};
				}
				if (value.JmpType != JmpType.JmpIfFalse || value.JmpPositon <= value.PCodePositon || !value.Condition.Contains("\u0001"))
				{
					continue;
				}
				string text2 = "";
				foreach (KeyValuePair<string, CodeArea> item in dictionary)
				{
					if (value.Condition.EndsWith(item.Key) || value.Condition.Contains(item.Key + " "))
					{
						text2 = item.Key;
						break;
					}
				}
				if (string.IsNullOrEmpty(text2))
				{
					continue;
				}
				if (value.Condition.Contains(string.Format(" <= {0} and ", text2)))
				{
					value.SCode = "case " + value.Condition.Replace(string.Format(" <= {0} and ", text2), " to ").Replace(string.Format(" >= {0}", text2), "");
				}
				else if (value.Condition.EndsWith(string.Format(" = {0}", text2)))
				{
					value.SCode = "case " + value.Condition.Replace(string.Format(" = {0}", text2), "");
				}
				else if (value.Condition.Contains(string.Format(" <= {0}", text2)))
				{
					value.SCode = "case is >= " + value.Condition.Replace(string.Format(" <= {0}", text2), "");
				}
				else if (value.Condition.Contains(string.Format(" >= {0}", text2)))
				{
					value.SCode = "case is <= " + value.Condition.Replace(string.Format(" >= {0}", text2), "");
				}
				else if (value.Condition.Contains(string.Format(" < {0}", text2)))
				{
					value.SCode = "case is > " + value.Condition.Replace(string.Format(" < {0}", text2), "");
				}
				else if (value.Condition.Contains(string.Format(" > {0}", text2)))
				{
					value.SCode = "case is < " + value.Condition.Replace(string.Format(" > {0}", text2), "");
				}
				value.JmpType = JmpType.None;
				CodeLine preCodeLine = value.PreCodeLine;
				while (string.IsNullOrEmpty(preCodeLine.SCode))
				{
					preCodeLine = preCodeLine.PreCodeLine;
				}
				if (preCodeLine.SCode == "case else ")
				{
					preCodeLine.SCode = "";
				}
				if (list[value.JmpPositon].PreCodeLine.JmpType == JmpType.Jmp)
				{
					list[value.JmpPositon].PreCodeLine.SCode = "case else ";
					list[value.JmpPositon].PreCodeLine.JmpType = JmpType.None;
					if (dictionary[text2].End == 0)
					{
						list[list[value.JmpPositon].PreCodeLine.JmpPositon].LabelSCode.Insert(0, "end choose ");
						dictionary[text2].End = list[list[value.JmpPositon].PreCodeLine.JmpPositon].PreCodeLine.PCodePositon;
					}
				}
				else if (dictionary[text2].End == 0)
				{
					list[value.JmpPositon].LabelSCode.Insert(0, "end choose ");
					dictionary[text2].End = list[value.JmpPositon].PreCodeLine.PCodePositon;
				}
			}
			areas.AddRange(dictionary.Values);
		}

		private static void ParseForNext(Dictionary<ushort, CodeLine> list, List<CodeArea> areas)
		{
			foreach (CodeLine value in list.Values)
			{
				if (string.IsNullOrEmpty(value.SCode) || value.JmpType != JmpType.JmpIfFalse || value.JmpPositon <= value.PCodePositon || list[value.JmpPositon].PreCodeLine.JmpType != JmpType.Jmp || list[value.JmpPositon].PreCodeLine.JmpPositon >= list[value.JmpPositon].PreCodeLine.PCodePositon || list[value.JmpPositon].PreCodeLine.JmpPositon >= value.PCodePositon)
				{
					continue;
				}
				CodeLine preCodeLine = list[list[value.JmpPositon].PreCodeLine.JmpPositon].PreCodeLine;
				if (preCodeLine == null || preCodeLine.JmpType != JmpType.Jmp)
				{
					continue;
				}
				string[] array = value.Condition.Split(new char[4] { '>', '<', '=', ' ' }, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length > 1)
				{
					CodeLine preCodeLine2 = value.PreCodeLine;
					while (string.IsNullOrEmpty(preCodeLine2.SCode))
					{
						preCodeLine2 = preCodeLine2.PreCodeLine;
					}
					string arg = "";
					if (preCodeLine2.SCode.StartsWith(string.Format("{0} += ", array[1])))
					{
						arg = "step " + preCodeLine2.SCode.Substring(string.Format("{0} += ", array[1]).Length);
					}
					preCodeLine2.SCode = "";
					value.SCode = string.Format("for {0} to {1} {2}", list[list[value.JmpPositon].PreCodeLine.JmpPositon].PreCodeLine.PreCodeLine.SCode, array[0], arg);
					value.JmpType = JmpType.None;
					list[list[value.JmpPositon].PreCodeLine.JmpPositon].PreCodeLine.PreCodeLine.SCode = "";
					list[list[value.JmpPositon].PreCodeLine.JmpPositon].PreCodeLine.SCode = "";
					list[list[value.JmpPositon].PreCodeLine.JmpPositon].PreCodeLine.JmpType = JmpType.None;
					list[value.JmpPositon].PreCodeLine.SCode = "next ";
					list[value.JmpPositon].PreCodeLine.JmpType = JmpType.None;
					areas.Add(new CodeArea
					{
						Type = "for",
						Start = value.PCodePositon,
						End = list[value.JmpPositon].PreCodeLine.PCodePositon
					});
				}
			}
		}

		private static void ParseDoLoop(Dictionary<ushort, CodeLine> list, List<CodeArea> areas)
		{
			foreach (CodeLine value in list.Values)
			{
				if (string.IsNullOrEmpty(value.SCode))
				{
					continue;
				}
				if (value.JmpType == JmpType.JmpIfFalse)
				{
					if (value.JmpPositon > value.PCodePositon)
					{
						if (list[value.JmpPositon].PreCodeLine.JmpType == JmpType.Jmp && list[value.JmpPositon].PreCodeLine.JmpPositon < list[value.JmpPositon].PreCodeLine.PCodePositon && list[value.JmpPositon].PreCodeLine.JmpPositon < value.PCodePositon)
						{
							value.SCode = string.Format("do while {0}", value.Condition);
							value.JmpType = JmpType.None;
							list[value.JmpPositon].PreCodeLine.SCode = "loop ";
							list[value.JmpPositon].PreCodeLine.JmpType = JmpType.None;
							areas.Add(new CodeArea
							{
								Type = "do",
								Start = value.PCodePositon,
								End = list[value.JmpPositon].PreCodeLine.PCodePositon
							});
						}
					}
					else
					{
						list[value.JmpPositon].LabelSCode.Add("do ");
						value.SCode = string.Format("loop until {0}", value.Condition);
						value.JmpType = JmpType.None;
						areas.Add(new CodeArea
						{
							Type = "do",
							Start = list[value.JmpPositon].PCodePositon,
							End = value.PCodePositon
						});
					}
				}
				else
				{
					if (value.JmpType != JmpType.JmpIfTrue)
					{
						continue;
					}
					if (value.JmpPositon > value.PCodePositon)
					{
						if (list[value.JmpPositon].PreCodeLine.JmpType == JmpType.Jmp && list[value.JmpPositon].PreCodeLine.JmpPositon < list[value.JmpPositon].PreCodeLine.PCodePositon && list[value.JmpPositon].PreCodeLine.JmpPositon < value.PCodePositon)
						{
							value.SCode = string.Format("do until {0}", value.Condition);
							value.JmpType = JmpType.None;
							list[value.JmpPositon].PreCodeLine.SCode = "loop ";
							list[value.JmpPositon].PreCodeLine.JmpType = JmpType.None;
							areas.Add(new CodeArea
							{
								Type = "do",
								Start = value.PCodePositon,
								End = list[value.JmpPositon].PreCodeLine.PCodePositon
							});
						}
					}
					else
					{
						list[value.JmpPositon].LabelSCode.Add("do ");
						value.SCode = string.Format("loop while {0}", value.Condition);
						value.JmpType = JmpType.None;
						areas.Add(new CodeArea
						{
							Type = "do",
							Start = list[value.JmpPositon].PCodePositon,
							End = value.PCodePositon
						});
					}
				}
			}
		}

		private static void ParseExitContinue(Dictionary<ushort, CodeLine> list, List<CodeArea> areas)
		{
			if (areas.Count == 0)
			{
				return;
			}
			foreach (CodeLine codeLine in list.Values)
			{
				if (string.IsNullOrEmpty(codeLine.SCode) || codeLine.JmpType != JmpType.Jmp || codeLine.JmpPositon <= codeLine.PCodePositon)
				{
					continue;
				}
				CodeArea codeArea = areas.OrderBy((CodeArea o) => Math.Abs(codeLine.PCodePositon - o.Start) + Math.Abs(o.End - codeLine.PCodePositon)).First();
				if (codeLine.PCodePositon >= codeArea.Start && codeLine.PCodePositon <= codeArea.End)
				{
					if (list[codeLine.JmpPositon].PreCodeLine.PCodePositon == codeArea.End)
					{
						codeLine.SCode = "exit";
						codeLine.JmpType = JmpType.None;
					}
					else if (list[codeLine.JmpPositon].PCodePositon == codeArea.End)
					{
						codeLine.SCode = "continue";
						codeLine.JmpType = JmpType.None;
					}
				}
			}
		}

		private static void ParseIfElse(Dictionary<ushort, CodeLine> list)
		{
			foreach (CodeLine value in list.Values)
			{
				if (string.IsNullOrEmpty(value.SCode) || value.JmpType != JmpType.JmpIfFalse || value.JmpPositon <= value.PCodePositon)
				{
					continue;
				}
				if (list[value.JmpPositon].PreCodeLine.JmpType == JmpType.Jmp && list[value.JmpPositon].PreCodeLine.JmpPositon > list[value.JmpPositon].PreCodeLine.PCodePositon)
				{
					value.SCode = string.Format("if {0} then ", value.Condition);
					value.JmpType = JmpType.None;
					if (list[value.JmpPositon].PreCodeLine.SCode == "exit" || list[value.JmpPositon].PreCodeLine.SCode == "continue")
					{
						list[value.JmpPositon].PreCodeLine.JmpType = JmpType.None;
						list[value.JmpPositon].LabelSCode.Insert(0, "end if ");
					}
					else
					{
						list[value.JmpPositon].PreCodeLine.SCode = "else ";
						list[value.JmpPositon].PreCodeLine.JmpType = JmpType.None;
						list[list[value.JmpPositon].PreCodeLine.JmpPositon].LabelSCode.Insert(0, "end if ");
					}
				}
				else
				{
					value.SCode = string.Format("if {0} then", value.Condition);
					value.JmpType = JmpType.None;
					list[value.JmpPositon].LabelSCode.Insert(0, "end if ");
				}
			}
			int num = 0;
			HashSet<int> hashSet = new HashSet<int>();
			foreach (CodeLine value2 in list.Values)
			{
				if (value2.SCode == null)
				{
					continue;
				}
				if (value2.SCode.Trim().StartsWith("if "))
				{
					num++;
					CodeLine firstVailiablePreCodeLine = GetFirstVailiablePreCodeLine(value2.PreCodeLine);
					if (((firstVailiablePreCodeLine != null) ? firstVailiablePreCodeLine.SCode : null) == "else ")
					{
						firstVailiablePreCodeLine.SCode += value2.SCode;
						value2.SCode = "";
						hashSet.Add(num);
					}
				}
				for (int i = 0; i < value2.LabelSCode.Count; i++)
				{
					if (value2.LabelSCode[i].Trim().StartsWith("end if"))
					{
						if (hashSet.Contains(num))
						{
							value2.LabelSCode[i] = "";
							hashSet.Remove(num);
						}
						num--;
					}
				}
				if (value2.SCode.Trim().StartsWith("end if"))
				{
					if (hashSet.Contains(num))
					{
						value2.SCode = "";
						hashSet.Remove(num);
					}
					num--;
				}
			}
		}

		private static void ParseGoto(Dictionary<ushort, CodeLine> list)
		{
			foreach (CodeLine value in list.Values)
			{
				if (!string.IsNullOrEmpty(value.SCode))
				{
					JmpType jmpType = value.JmpType;
					int num = 1;
				}
			}
		}

		private static void ParseEventReturn(Dictionary<ushort, CodeLine> list)
		{
			foreach (CodeLine value in list.Values)
			{
				if (!value.SCode.Trim().StartsWith("if isvalid(::message) then goto "))
				{
					continue;
				}
				CodeLine firstVailiableNextCodeLine = GetFirstVailiableNextCodeLine(value);
				if (firstVailiableNextCodeLine == null || !firstVailiableNextCodeLine.SCode.Trim().Equals("return 0"))
				{
					continue;
				}
				CodeLine firstVailiableNextCodeLine2 = GetFirstVailiableNextCodeLine(firstVailiableNextCodeLine);
				if (firstVailiableNextCodeLine2 != null && firstVailiableNextCodeLine2.SCode.Trim().StartsWith("goto "))
				{
					CodeLine firstVailiableNextCodeLine3 = GetFirstVailiableNextCodeLine(firstVailiableNextCodeLine2);
					if (firstVailiableNextCodeLine3 != null && firstVailiableNextCodeLine3.SCode.Trim().Equals("return ::message.returnvalue"))
					{
						value.SCode = "";
						firstVailiableNextCodeLine.SCode = "";
						firstVailiableNextCodeLine2.SCode = "";
						firstVailiableNextCodeLine3.SCode = "";
					}
				}
			}
		}

		private static void ParseIndent(PbFunction pbFunction, Dictionary<ushort, CodeLine> list)
		{
			int indent = 0;
			foreach (CodeLine value in list.Values)
			{
				try
				{
					for (int i = 0; i < value.LabelSCode.Count; i++)
					{
						value.LabelSCode[i] = ParseIndent(value.LabelSCode[i], ref indent);
					}
					if (!string.IsNullOrEmpty(value.SCode))
					{
						value.SCode = ParseIndent(value.SCode, ref indent);
					}
				}
				catch (Exception)
				{
					break;
				}
			}
		}

		private static string ParseIndent(string scode, ref int indent)
		{
			if (scode.StartsWith("try "))
			{
				scode = "".PadRight(indent * 4) + scode;
				indent++;
			}
			else if (scode.StartsWith("catch "))
			{
				indent--;
				scode = "".PadRight(indent * 4) + scode;
				indent++;
			}
			else if (scode.StartsWith("finally "))
			{
				indent--;
				scode = "".PadRight(indent * 4) + scode;
				indent++;
			}
			else if (scode.StartsWith("end try "))
			{
				indent--;
				scode = "".PadRight(indent * 4) + scode;
			}
			else if (scode.StartsWith("if "))
			{
				scode = "".PadRight(indent * 4) + scode;
				indent++;
			}
			else if (scode.StartsWith("else "))
			{
				indent--;
				scode = "".PadRight(indent * 4) + scode;
				indent++;
			}
			else if (scode.StartsWith("end if "))
			{
				indent--;
				scode = "".PadRight(indent * 4) + scode;
			}
			else if (scode.StartsWith("for "))
			{
				scode = "".PadRight(indent * 4) + scode;
				indent++;
			}
			else if (scode.StartsWith("next "))
			{
				indent--;
				scode = "".PadRight(indent * 4) + scode;
			}
			else if (scode.StartsWith("choose case "))
			{
				scode = "".PadRight(indent * 4) + scode;
				indent++;
				indent++;
			}
			else if (scode.StartsWith("case "))
			{
				indent--;
				scode = "".PadRight(indent * 4) + scode;
				indent++;
			}
			else if (scode.StartsWith("end choose "))
			{
				indent--;
				indent--;
				scode = "".PadRight(indent * 4) + scode;
			}
			else if (scode.StartsWith("do "))
			{
				scode = "".PadRight(indent * 4) + scode;
				indent++;
			}
			else if (scode.StartsWith("loop "))
			{
				indent--;
				scode = "".PadRight(indent * 4) + scode;
			}
			else if (!string.IsNullOrEmpty(scode))
			{
				scode = "".PadRight(indent * 4) + scode;
			}
			return scode;
		}

		private static void Init(ushort version)
		{
			if (!Count.ContainsKey(version))
			{
				Count[version] = 0;
				Goodcount[version] = 0;
				UsedPCodeList[version] = new HashSet<ushort>();
				UnParsedPCodeList[version] = new HashSet<ushort>();
			}
		}

		private static PCodeParserBase GetPCodeParse(PbFunction pbFunction, ushort version)
		{
			PCodeParserBase result = null;
			switch (version)
			{
			case 79:
			case 114:
			case 146:
			case 166:
			case 193:
			case 196:
				result = new PCodeParser90(pbFunction);
				break;
			case 238:
				result = new PCodeParser100(pbFunction);
				break;
			case 283:
				result = new PCodeParser105(pbFunction);
				break;
			case 316:
			case 319:
			case 321:
			case 322:
			case 325:
			case 333:
			case 334:
				result = new PCodeParser110(pbFunction);
				break;
			}
			return result;
		}

		private static CodeLine GetFirstVailiablePreCodeLine(CodeLine codeLine)
		{
			CodeLine preCodeLine = codeLine.PreCodeLine;
			while (preCodeLine != null && string.IsNullOrEmpty(preCodeLine.SCode))
			{
				preCodeLine = preCodeLine.PreCodeLine;
			}
			return preCodeLine;
		}

		private static CodeLine GetFirstVailiableNextCodeLine(CodeLine codeLine)
		{
			CodeLine nextCodeLine = codeLine.NextCodeLine;
			while (nextCodeLine != null && string.IsNullOrEmpty(nextCodeLine.SCode))
			{
				nextCodeLine = nextCodeLine.NextCodeLine;
			}
			return nextCodeLine;
		}
	}
}
