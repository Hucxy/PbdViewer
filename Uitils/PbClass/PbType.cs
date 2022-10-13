using System;
using System.Collections.Generic;
using System.Linq;

namespace PbdViewer.Uitils.PbClass
{
	public class PbType
	{
		private static readonly Dictionary<ushort, PbType> ValueTypes = new Dictionary<ushort, PbType>();

		private bool _isFinded;

		public ushort Index { get; set; }

		public string Name { get; set; }

		public bool IsValueType { get; set; }

		public bool IsSystemType { get; set; }

		public bool IsReferencedObject { get; set; }

		public PbEnum Enum { get; set; }

		public PbEntry Entry { get; set; }

		public PbObject Object { get; set; }

		public PbType(PbEntry entry, ushort index, string name, bool isReferencedObject, bool isSystemEntry)
		{
			Name = name;
			Entry = entry;
			IsReferencedObject = isReferencedObject;
			if (isSystemEntry)
			{
				if (IsReferencedObject)
				{
					throw new Exception("system entry can't reference other object");
				}
				IsSystemType = true;
				Index = (ushort)(0x4000u | index);
				entry.Project.OnNewSystemType(this);
				return;
			}
			if (IsReferencedObject)
			{
				Enum = entry.Project.Enums.Values.FirstOrDefault((PbEnum o) => o.Name == Name);
			}
			Index = (ushort)(0x8000u | index);
			entry.OnNewType(this);
		}

		private PbType(ushort index)
		{
			Index = index;
			Name = GetValueTypeName(index);
			IsValueType = true;
		}

		public static PbType GetPbType(PbEntry pbEntry, ushort index)
		{
			switch (index >> 12)
			{
			case 0:
				if (!ValueTypes.ContainsKey(index))
				{
					ValueTypes[index] = new PbType(index);
				}
				return ValueTypes[index];
			case 4:
				return pbEntry.Project.SystemTypes[index];
			case 8:
				return pbEntry.Types[index];
			case 12:
				return new PbType(0);
			default:
				throw new Exception(string.Format("Unkown Type {0:X4}", index));
			}
		}

		private string GetValueTypeName(int low)
		{
			switch (low)
			{
			case 0:
				return "";
			case 1:
				return "integer";
			case 2:
				return "long";
			case 3:
				return "real";
			case 4:
				return "double";
			case 5:
				return "decimal";
			case 6:
				return "string";
			case 7:
				return "boolean";
			case 8:
				return "any";
			case 9:
				return "uint";
			case 10:
				return "ulong";
			case 11:
				return "blob";
			case 12:
				return "date";
			case 13:
				return "time";
			case 14:
				return "datetime";
			case 15:
				return "cursor";
			case 16:
				return "procedure";
			case 18:
				return "char";
			case 19:
				return "objhandle";
			case 20:
				return "longlong";
			case 21:
				return "byte";
			default:
				return string.Format("{0:X4}", low);
			}
		}

		public PbObject GetObject(PbEntry pbEntry)
		{
			if (Object != null)
			{
				return Object;
			}
			if (IsValueType)
			{
				return Object;
			}
			if (_isFinded)
			{
				return Object;
			}
			if (Name.Contains('`') || IsSystemType || IsReferencedObject)
			{
				if (pbEntry.Project.Objects.ContainsKey(Name))
				{
					Object = pbEntry.Project.Objects[Name];
				}
			}
			else
			{
				pbEntry = Entry ?? pbEntry;
				if (pbEntry.Objects.ContainsKey(Index))
				{
					Object = pbEntry.Objects[Index];
				}
			}
			_isFinded = true;
			return Object;
		}
	}
}
