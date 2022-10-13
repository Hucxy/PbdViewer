using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace PbdViewer.Uitils.PbClass
{
	public class PbProject
	{
		private readonly string _filePath;

		private readonly string _dir;

		public bool IsDebug;

		[CompilerGenerated]
		private readonly List<PbFile> _003CFiles_003Ek__BackingField = new List<PbFile>();

		[CompilerGenerated]
		private readonly Dictionary<string, PbObject> _003CObjects_003Ek__BackingField = new Dictionary<string, PbObject>();

		[CompilerGenerated]
		private readonly Dictionary<ushort, PbType> _003CSystemTypes_003Ek__BackingField = new Dictionary<ushort, PbType>();

		[CompilerGenerated]
		private readonly Dictionary<ushort, PbEnum> _003CEnums_003Ek__BackingField = new Dictionary<ushort, PbEnum>();

		public List<PbFile> Files
		{
			[CompilerGenerated]
			get
			{
				return _003CFiles_003Ek__BackingField;
			}
		}

		public Dictionary<string, PbObject> Objects
		{
			[CompilerGenerated]
			get
			{
				return _003CObjects_003Ek__BackingField;
			}
		}

		public Dictionary<ushort, PbType> SystemTypes
		{
			[CompilerGenerated]
			get
			{
				return _003CSystemTypes_003Ek__BackingField;
			}
		}

		public Dictionary<ushort, PbEnum> Enums
		{
			[CompilerGenerated]
			get
			{
				return _003CEnums_003Ek__BackingField;
			}
		}

		public PbEntry SystemEntry { get; set; }

		public bool IsUnicode { get; set; }

		public bool IsPb5 { get; set; }

		public ushort Version { get; set; }

		public PbProject(string filePath)
		{
			_filePath = filePath;
			_dir = Path.GetDirectoryName(filePath);
			PbFile item = new PbFile(this, filePath);
			Files.Insert(0, item);
			foreach (PbFile file in Files)
			{
				foreach (PbEntry entry in file.Entries)
				{
					entry.ParseObject();
				}
			}
			PbEntry systemEntry = SystemEntry;
			if (systemEntry != null)
			{
				systemEntry.ParseInherit();
			}
			foreach (PbFile file2 in Files)
			{
				foreach (PbEntry entry2 in file2.Entries)
				{
					entry2.ParseInherit();
				}
			}
		}

		public string GetString(byte[] buffer, int offset, int size)
		{
			if (!IsUnicode)
			{
				return Encoding.Default.GetString(buffer, offset, size);
			}
			return Encoding.Unicode.GetString(buffer, offset, size);
		}

		public string GetString(byte[] buffer)
		{
			if (!IsUnicode)
			{
				return Encoding.Default.GetString(buffer);
			}
			return Encoding.Unicode.GetString(buffer);
		}

		public void OnNewLibrary(string libpath, bool isFullPath)
		{
			if (!isFullPath)
			{
				libpath = Path.Combine(_dir, libpath);
			}
			if (!string.Equals(_filePath, libpath, StringComparison.OrdinalIgnoreCase) && File.Exists(libpath))
			{
				Files.Add(new PbFile(this, libpath));
			}
		}

		public PbFile OnSystemLibrary(ushort version)
		{
			if (Version == 0)
			{
				Version = version;
				return new PbFile(this, Version);
			}
			if (Version != version)
			{
				throw new Exception("two version library in one project??");
			}
			return null;
		}

		public void OnSystemEntry(PbEntry pbEntry)
		{
			SystemEntry = pbEntry;
		}

		public void OnNewSystemType(PbType pbType)
		{
			SystemTypes[pbType.Index] = pbType;
		}

		public void OnNewObject(PbObject pbObject, string name = null)
		{
			Objects[name ?? pbObject.Type.Name] = pbObject;
		}

		public void OnNewEnumItem(PbType type, ushort index, string itemName)
		{
			if (!Enums.ContainsKey(type.Index))
			{
				Enums[type.Index] = new PbEnum
				{
					Index = type.Index,
					Name = type.Name
				};
			}
			Enums[type.Index].Items[index] = itemName + "!";
		}
	}
}
