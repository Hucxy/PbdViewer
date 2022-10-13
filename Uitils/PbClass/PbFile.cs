using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;

namespace PbdViewer.Uitils.PbClass
{
	public class PbFile
	{
		[CompilerGenerated]
		private readonly PbProject _003CProject_003Ek__BackingField;

		[CompilerGenerated]
		private readonly string _003CFilePath_003Ek__BackingField;

		[CompilerGenerated]
		private readonly string _003CFileName_003Ek__BackingField;

		[CompilerGenerated]
		private readonly List<PbEntry> _003CEntries_003Ek__BackingField = new List<PbEntry>();

		public PbProject Project
		{
			[CompilerGenerated]
			get
			{
				return _003CProject_003Ek__BackingField;
			}
		}

		public string FilePath
		{
			[CompilerGenerated]
			get
			{
				return _003CFilePath_003Ek__BackingField;
			}
		}

		public string FileName
		{
			[CompilerGenerated]
			get
			{
				return _003CFileName_003Ek__BackingField;
			}
		}

		public List<PbEntry> Entries
		{
			[CompilerGenerated]
			get
			{
				return _003CEntries_003Ek__BackingField;
			}
		}

		public PbFile(PbProject project, string filePath)
		{
			_003CProject_003Ek__BackingField = project;
			_003CFilePath_003Ek__BackingField = filePath;
			_003CFileName_003Ek__BackingField = Path.GetFileName(FilePath);
			FileStream fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
			foreach (long node in GetNodeList(fileStream))
			{
				byte[] buffer = new byte[32];
				fileStream.Seek(node, SeekOrigin.Begin);
				fileStream.Read(buffer, 0, 32);
				long current = fileStream.Position;
				ushort uShort = BufferHelper.GetUShort(buffer, 20L);
				int num = ((!Project.IsUnicode) ? 1 : 2);
				int num2 = 4 + num * 4;
				int num3 = num2 + 16;
				byte[] array = new byte[num3];
				for (int i = 0; i < uShort; i++)
				{
					fileStream.Seek(current, SeekOrigin.Begin);
					fileStream.Read(array, 0, num3);
					if (!Encoding.ASCII.GetString(array, 0, 4).Equals("ENT*") || (!Project.GetString(array, 4, num * 4).Equals("0600") && !Project.GetString(array, 4, num * 4).Equals("0500")))
					{
						throw new Exception("格式错误");
					}
					uint uInt = BufferHelper.GetUInt(array, num2);
					uint uInt2 = BufferHelper.GetUInt(array, num2 + 4);
					ushort uShort2 = BufferHelper.GetUShort(array, num2 + 14);
					byte[] buffer2 = new byte[uShort2];
					fileStream.Read(buffer2, 0, uShort2);
					current = fileStream.Position;
					string @string = Project.GetString(buffer2, 0, uShort2 - num);
					Entries.Add(new PbEntry(this, @string, ReadData(fileStream, uInt, uInt2)));
				}
			}
			fileStream.Close();
		}

		public PbFile(PbProject project, ushort version)
		{
			_003CProject_003Ek__BackingField = project;
			_003CFileName_003Ek__BackingField = "system";
			UnmanagedMemoryStream stream = new ResourceManager("PbdViewer.g", GetType().Assembly).GetStream(string.Format("Resoures/{0:X4}.bin", version).ToLower());
			if (stream != null)
			{
				byte[] array = new byte[4096];
				GZipStream gZipStream = new GZipStream(stream, CompressionMode.Decompress, true);
				MemoryStream memoryStream = new MemoryStream();
				int num;
				do
				{
					num = gZipStream.Read(array, 0, array.Length);
					memoryStream.Write(array, 0, num);
				}
				while (num > 0);
				gZipStream.Close();
				stream.Close();
				byte[] entryData = memoryStream.ToArray();
				memoryStream.Close();
				Entries.Add(new PbEntry(this, "_typedef.grp", entryData));
			}
		}

		private static byte[] ReadData(Stream stream, long start, long size)
		{
			byte[] array = new byte[size];
			int num = 0;
			long offset = start;
			byte[] array2 = new byte[10];
			while (num < size)
			{
				stream.Seek(offset, SeekOrigin.Begin);
				stream.Read(array2, 0, 10);
				if (!Encoding.ASCII.GetString(array2, 0, 4).Equals("DAT*"))
				{
					break;
				}
				ushort uShort = BufferHelper.GetUShort(array2, 8L);
				stream.Read(array, num, uShort);
				num += uShort;
				offset = BufferHelper.GetUInt(array2, 4L);
			}
			return array;
		}

		private IEnumerable<long> GetNodeList(Stream stream)
		{
			List<long> list = new List<long>();
			byte[] array = new byte[512];
			long num = 0L;
			bool flag = false;
			stream.Seek(0L, SeekOrigin.Begin);
			for (int num2 = stream.Read(array, 0, 512); num2 > 0; num2 = stream.Read(array, 0, 512))
			{
				if (Encoding.ASCII.GetString(array, 0, 4).Equals("HDR*"))
				{
					if (Encoding.ASCII.GetString(array, 4, 12).Equals("PowerBuilder"))
					{
						if (Encoding.ASCII.GetString(array, 18, 4).Equals("0500"))
						{
							Project.IsPb5 = true;
							flag = true;
							break;
						}
						if (Encoding.ASCII.GetString(array, 18, 4).Equals("0600"))
						{
							flag = true;
							break;
						}
					}
					if (Encoding.Unicode.GetString(array, 4, 24).Equals("PowerBuilder") && Encoding.Unicode.GetString(array, 32, 8).Equals("0600"))
					{
						flag = true;
						Project.IsUnicode = true;
						break;
					}
				}
				num += num2;
			}
			if (flag)
			{
				num += (Project.IsUnicode ? 1536 : 1024);
				stream.Seek(num, SeekOrigin.Begin);
				int num2 = stream.Read(array, 0, 512);
				if (num2 != 512 || !Encoding.ASCII.GetString(array, 0, 4).Equals("NOD*"))
				{
					throw new Exception("格式错误");
				}
				list.Add(num);
				uint uInt = BufferHelper.GetUInt(array, 4L);
				uint uInt2 = BufferHelper.GetUInt(array, 12L);
				while (uInt != 0)
				{
					stream.Seek(uInt, SeekOrigin.Begin);
					num2 = stream.Read(array, 0, 512);
					if (num2 != 512 || !Encoding.ASCII.GetString(array, 0, 4).Equals("NOD*"))
					{
						throw new Exception("格式错误");
					}
					list.Add(uInt);
					uInt = BufferHelper.GetUInt(array, 4L);
				}
				while (uInt2 != 0)
				{
					stream.Seek(uInt2, SeekOrigin.Begin);
					num2 = stream.Read(array, 0, 512);
					if (num2 != 512 || !Encoding.ASCII.GetString(array, 0, 4).Equals("NOD*"))
					{
						throw new Exception("格式错误");
					}
					list.Add(uInt2);
					uInt2 = BufferHelper.GetUInt(array, 12L);
				}
			}
			return list;
		}
	}
}
