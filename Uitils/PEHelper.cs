using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;

namespace PbdViewer.Uitils
{
	public class PEHelper
	{
		private class DosHeader
		{
			public byte[] e_magic = new byte[2];

			public byte[] e_cblp = new byte[2];

			public byte[] e_cp = new byte[2];

			public byte[] e_crlc = new byte[2];

			public byte[] e_cparhdr = new byte[2];

			public byte[] e_minalloc = new byte[2];

			public byte[] e_maxalloc = new byte[2];

			public byte[] e_ss = new byte[2];

			public byte[] e_sp = new byte[2];

			public byte[] e_csum = new byte[2];

			public byte[] e_ip = new byte[2];

			public byte[] e_cs = new byte[2];

			public byte[] e_rva = new byte[2];

			public byte[] e_fg = new byte[2];

			public byte[] e_bl1 = new byte[8];

			public byte[] e_oemid = new byte[2];

			public byte[] e_oeminfo = new byte[2];

			public byte[] e_bl2 = new byte[20];

			public byte[] e_PESTAR = new byte[2];

			public long FileStarIndex;

			public long FileEndIndex;
		}

		private class DosStub
		{
			public byte[] DosStubData;

			public long FileStarIndex;

			public long FileEndIndex;

			public DosStub(long Size)
			{
				DosStubData = new byte[Size];
			}
		}

		private class PEHeader
		{
			public byte[] Header = new byte[4];

			public byte[] Machine = new byte[2];

			public byte[] NumberOfSections = new byte[2];

			public byte[] TimeDateStamp = new byte[4];

			public byte[] PointerToSymbolTable = new byte[4];

			public byte[] NumberOfSymbols = new byte[4];

			public byte[] SizeOfOptionalHeader = new byte[2];

			public byte[] Characteristics = new byte[2];

			public long FileStarIndex;

			public long FileEndIndex;
		}

		private class OptionalHeader
		{
			public byte[] Magic = new byte[2];

			public byte[] MajorLinkerVersion = new byte[1];

			public byte[] MinorLinkerVersion = new byte[1];

			public byte[] SizeOfCode = new byte[4];

			public byte[] SizeOfInitializedData = new byte[4];

			public byte[] SizeOfUninitializedData = new byte[4];

			public byte[] AddressOfEntryPoint = new byte[4];

			public byte[] BaseOfCode = new byte[4];

			public byte[] ImageBase = new byte[4];

			public byte[] ImageFileCode = new byte[4];

			public byte[] SectionAlign = new byte[4];

			public byte[] FileAlign = new byte[4];

			public byte[] MajorOSV = new byte[2];

			public byte[] MinorOSV = new byte[2];

			public byte[] MajorImageVer = new byte[2];

			public byte[] MinorImageVer = new byte[2];

			public byte[] MajorSV = new byte[2];

			public byte[] MinorSV = new byte[2];

			public byte[] UNKNOW = new byte[4];

			public byte[] SizeOfImage = new byte[4];

			public byte[] SizeOfHeards = new byte[4];

			public byte[] CheckSum = new byte[4];

			public byte[] Subsystem = new byte[2];

			public byte[] DLL_Characteristics = new byte[2];

			public byte[] Bsize = new byte[4];

			public byte[] TimeBsize = new byte[4];

			public byte[] AucBsize = new byte[4];

			public byte[] SizeOfBsize = new byte[4];

			public byte[] FuckBsize = new byte[4];

			public byte[] DirectCount = new byte[4];

			public long FileStarIndex;

			public long FileEndIndex;
		}

		private class OptionalDirAttrib
		{
			public class DirAttrib
			{
				public byte[] DirRva = new byte[4];

				public byte[] DirSize = new byte[4];
			}

			public ArrayList DirByte = new ArrayList();

			public long FileStarIndex;

			public long FileEndIndex;
		}

		private class SectionTable
		{
			public class SectionData
			{
				public byte[] SectName = new byte[8];

				public byte[] VirtualAddress = new byte[4];

				public byte[] SizeOfRawDataRVA = new byte[4];

				public byte[] SizeOfRawDataSize = new byte[4];

				public byte[] PointerToRawData = new byte[4];

				public byte[] PointerToRelocations = new byte[4];

				public byte[] PointerToLinenumbers = new byte[4];

				public byte[] NumberOfRelocations = new byte[2];

				public byte[] NumberOfLinenumbers = new byte[2];

				public byte[] Characteristics = new byte[4];
			}

			public ArrayList Section = new ArrayList();

			public long FileStarIndex;

			public long FileEndIndex;
		}

		private class ExportDirectory
		{
			public byte[] Characteristics = new byte[4];

			public byte[] TimeDateStamp = new byte[4];

			public byte[] MajorVersion = new byte[2];

			public byte[] MinorVersion = new byte[2];

			public byte[] Name = new byte[4];

			public byte[] Base = new byte[4];

			public byte[] NumberOfFunctions = new byte[4];

			public byte[] NumberOfNames = new byte[4];

			public byte[] AddressOfFunctions = new byte[4];

			public byte[] AddressOfNames = new byte[4];

			public byte[] AddressOfNameOrdinals = new byte[4];

			public ArrayList AddressOfFunctionsList = new ArrayList();

			public ArrayList AddressOfNamesList = new ArrayList();

			public ArrayList AddressOfNameOrdinalsList = new ArrayList();

			public ArrayList NameList = new ArrayList();

			public long FileStarIndex;

			public long FileEndIndex;
		}

		private class ImportDirectory
		{
			public class ImportDate
			{
				public class FunctionList
				{
					public byte[] OriginalFirst = new byte[4];

					public byte[] FunctionName;

					public byte[] FunctionHead = new byte[2];
				}

				public byte[] OriginalFirstThunk = new byte[4];

				public byte[] TimeDateStamp = new byte[4];

				public byte[] ForwarderChain = new byte[4];

				public byte[] Name = new byte[4];

				public byte[] FirstThunk = new byte[4];

				public byte[] DLLName;

				public ArrayList DllFunctionList = new ArrayList();
			}

			public ArrayList ImportList = new ArrayList();

			public long FileStarIndex;

			public long FileEndIndex;
		}

		private class ResourceDirectory
		{
			public class DirectoryEntry
			{
				public class DataEntry
				{
					public byte[] ResourRVA = new byte[4];

					public byte[] ResourSize = new byte[4];

					public byte[] ResourTest = new byte[4];

					public byte[] ResourWen = new byte[4];

					public long FileStarIndex;

					public long FileEndIndex;
				}

				public byte[] Name = new byte[4];

				public byte[] Id = new byte[4];

				public ArrayList DataEntryList = new ArrayList();

				public ArrayList NodeDirectoryList = new ArrayList();
			}

			public byte[] Characteristics = new byte[4];

			public byte[] TimeDateStamp = new byte[4];

			public byte[] MajorVersion = new byte[2];

			public byte[] MinorVersion = new byte[2];

			public byte[] NumberOfNamedEntries = new byte[2];

			public byte[] NumberOfIdEntries = new byte[2];

			public byte[] Name;

			public ArrayList EntryList = new ArrayList();

			public long FileStarIndex;

			public long FileEndIndex;
		}

		private byte[] PEFileByte;

		private bool _OpenFile;

		private long PEFileIndex;

		private DosHeader _DosHeader;

		private DosStub _DosStub;

		private PEHeader _PEHeader;

		private OptionalHeader _OptionalHeader;

		private OptionalDirAttrib _OptionalDirAttrib;

		private SectionTable _SectionTable;

		private ExportDirectory _ExportDirectory;

		private ImportDirectory _ImportDirectory;

		private ResourceDirectory _ResourceDirectory;

		public bool OpenFile
		{
			get
			{
				return _OpenFile;
			}
		}

		public PEHelper(Stream stream)
		{
			_OpenFile = false;
			PEFileByte = new byte[stream.Length];
			long position = stream.Position;
			stream.Seek(0L, SeekOrigin.Begin);
			stream.Read(PEFileByte, 0, PEFileByte.Length);
			stream.Seek(position, SeekOrigin.Begin);
			LoadFile();
			_OpenFile = true;
		}

		public PEHelper(string FileName)
		{
			_OpenFile = false;
			FileStream fileStream = new FileStream(FileName, FileMode.Open);
			PEFileByte = new byte[fileStream.Length];
			fileStream.Read(PEFileByte, 0, PEFileByte.Length);
			fileStream.Close();
			LoadFile();
			_OpenFile = true;
		}

		public uint GetOffset(uint offset)
		{
			foreach (SectionTable.SectionData item in _SectionTable.Section)
			{
				uint uInt = BufferHelper.GetUInt(item.VirtualAddress, 0L);
				uint num = BufferHelper.GetUInt(item.SizeOfRawDataRVA, 0L) + BufferHelper.GetUInt(_OptionalHeader.ImageFileCode, 0L);
				if (offset >= num && offset <= num + uInt)
				{
					return BufferHelper.GetUInt(item.PointerToRawData, 0L) + (offset - num);
				}
			}
			return uint.MaxValue;
		}

		private void LoadFile()
		{
			LoadDosHeader();
			LoadDosStub();
			LoadPEHeader();
			LoadOptionalHeader();
			LoadOptionalDirAttrib();
			LoadSectionTable();
			LoadExportDirectory();
			LoadImportDirectory();
			LoadResourceDirectory();
		}

		private void LoadDosHeader()
		{
			_DosHeader = new DosHeader();
			_DosHeader.FileStarIndex = PEFileIndex;
			Loadbyte(ref _DosHeader.e_magic);
			Loadbyte(ref _DosHeader.e_cblp);
			Loadbyte(ref _DosHeader.e_cp);
			Loadbyte(ref _DosHeader.e_crlc);
			Loadbyte(ref _DosHeader.e_cparhdr);
			Loadbyte(ref _DosHeader.e_minalloc);
			Loadbyte(ref _DosHeader.e_maxalloc);
			Loadbyte(ref _DosHeader.e_ss);
			Loadbyte(ref _DosHeader.e_sp);
			Loadbyte(ref _DosHeader.e_csum);
			Loadbyte(ref _DosHeader.e_ip);
			Loadbyte(ref _DosHeader.e_cs);
			Loadbyte(ref _DosHeader.e_rva);
			Loadbyte(ref _DosHeader.e_fg);
			Loadbyte(ref _DosHeader.e_bl1);
			Loadbyte(ref _DosHeader.e_oemid);
			Loadbyte(ref _DosHeader.e_oeminfo);
			Loadbyte(ref _DosHeader.e_bl2);
			Loadbyte(ref _DosHeader.e_PESTAR);
			_DosHeader.FileEndIndex = PEFileIndex;
		}

		private void LoadDosStub()
		{
			long size = GetLong(_DosHeader.e_PESTAR) - PEFileIndex;
			_DosStub = new DosStub(size);
			_DosStub.FileStarIndex = PEFileIndex;
			Loadbyte(ref _DosStub.DosStubData);
			_DosStub.FileEndIndex = PEFileIndex;
		}

		private void LoadPEHeader()
		{
			_PEHeader = new PEHeader();
			_PEHeader.FileStarIndex = PEFileIndex;
			Loadbyte(ref _PEHeader.Header);
			Loadbyte(ref _PEHeader.Machine);
			Loadbyte(ref _PEHeader.NumberOfSections);
			Loadbyte(ref _PEHeader.TimeDateStamp);
			Loadbyte(ref _PEHeader.PointerToSymbolTable);
			Loadbyte(ref _PEHeader.NumberOfSymbols);
			Loadbyte(ref _PEHeader.SizeOfOptionalHeader);
			Loadbyte(ref _PEHeader.Characteristics);
			_PEHeader.FileEndIndex = PEFileIndex;
		}

		private void LoadOptionalHeader()
		{
			_OptionalHeader = new OptionalHeader();
			_OptionalHeader.FileStarIndex = PEFileIndex;
			Loadbyte(ref _OptionalHeader.Magic);
			Loadbyte(ref _OptionalHeader.MajorLinkerVersion);
			Loadbyte(ref _OptionalHeader.MinorLinkerVersion);
			Loadbyte(ref _OptionalHeader.SizeOfCode);
			Loadbyte(ref _OptionalHeader.SizeOfInitializedData);
			Loadbyte(ref _OptionalHeader.SizeOfUninitializedData);
			Loadbyte(ref _OptionalHeader.AddressOfEntryPoint);
			Loadbyte(ref _OptionalHeader.BaseOfCode);
			Loadbyte(ref _OptionalHeader.ImageBase);
			Loadbyte(ref _OptionalHeader.ImageFileCode);
			Loadbyte(ref _OptionalHeader.SectionAlign);
			Loadbyte(ref _OptionalHeader.FileAlign);
			Loadbyte(ref _OptionalHeader.MajorOSV);
			Loadbyte(ref _OptionalHeader.MinorOSV);
			Loadbyte(ref _OptionalHeader.MajorImageVer);
			Loadbyte(ref _OptionalHeader.MinorImageVer);
			Loadbyte(ref _OptionalHeader.MajorSV);
			Loadbyte(ref _OptionalHeader.MinorSV);
			Loadbyte(ref _OptionalHeader.UNKNOW);
			Loadbyte(ref _OptionalHeader.SizeOfImage);
			Loadbyte(ref _OptionalHeader.SizeOfHeards);
			Loadbyte(ref _OptionalHeader.CheckSum);
			Loadbyte(ref _OptionalHeader.Subsystem);
			Loadbyte(ref _OptionalHeader.DLL_Characteristics);
			Loadbyte(ref _OptionalHeader.Bsize);
			Loadbyte(ref _OptionalHeader.TimeBsize);
			Loadbyte(ref _OptionalHeader.AucBsize);
			Loadbyte(ref _OptionalHeader.SizeOfBsize);
			Loadbyte(ref _OptionalHeader.FuckBsize);
			Loadbyte(ref _OptionalHeader.DirectCount);
			_OptionalHeader.FileEndIndex = PEFileIndex;
		}

		private void LoadOptionalDirAttrib()
		{
			_OptionalDirAttrib = new OptionalDirAttrib();
			_OptionalDirAttrib.FileStarIndex = PEFileIndex;
			long @long = GetLong(_OptionalHeader.DirectCount);
			for (int i = 0; i != @long; i++)
			{
				OptionalDirAttrib.DirAttrib dirAttrib = new OptionalDirAttrib.DirAttrib();
				Loadbyte(ref dirAttrib.DirRva);
				Loadbyte(ref dirAttrib.DirSize);
				_OptionalDirAttrib.DirByte.Add(dirAttrib);
			}
			_OptionalDirAttrib.FileEndIndex = PEFileIndex;
		}

		private void LoadSectionTable()
		{
			_SectionTable = new SectionTable();
			long @long = GetLong(_PEHeader.NumberOfSections);
			_SectionTable.FileStarIndex = PEFileIndex;
			for (long num = 0L; num != @long; num++)
			{
				SectionTable.SectionData sectionData = new SectionTable.SectionData();
				Loadbyte(ref sectionData.SectName);
				Loadbyte(ref sectionData.VirtualAddress);
				Loadbyte(ref sectionData.SizeOfRawDataRVA);
				Loadbyte(ref sectionData.SizeOfRawDataSize);
				Loadbyte(ref sectionData.PointerToRawData);
				Loadbyte(ref sectionData.PointerToRelocations);
				Loadbyte(ref sectionData.PointerToLinenumbers);
				Loadbyte(ref sectionData.NumberOfRelocations);
				Loadbyte(ref sectionData.NumberOfLinenumbers);
				Loadbyte(ref sectionData.Characteristics);
				_SectionTable.Section.Add(sectionData);
			}
			_SectionTable.FileEndIndex = PEFileIndex;
		}

		private void LoadExportDirectory()
		{
			if (_OptionalDirAttrib.DirByte.Count == 0)
			{
				return;
			}
			OptionalDirAttrib.DirAttrib dirAttrib = (OptionalDirAttrib.DirAttrib)_OptionalDirAttrib.DirByte[0];
			if (GetLong(dirAttrib.DirRva) == 0L)
			{
				return;
			}
			long @long = GetLong(dirAttrib.DirRva);
			_ExportDirectory = new ExportDirectory();
			for (int i = 0; i != _SectionTable.Section.Count; i++)
			{
				SectionTable.SectionData sectionData = (SectionTable.SectionData)_SectionTable.Section[i];
				long long2 = GetLong(sectionData.SizeOfRawDataRVA);
				long long3 = GetLong(sectionData.SizeOfRawDataSize);
				if (@long < long2 || @long >= long2 + long3)
				{
					continue;
				}
				PEFileIndex = @long - GetLong(sectionData.SizeOfRawDataRVA) + GetLong(sectionData.PointerToRawData);
				_ExportDirectory.FileStarIndex = PEFileIndex;
				_ExportDirectory.FileEndIndex = PEFileIndex + GetLong(dirAttrib.DirSize);
				Loadbyte(ref _ExportDirectory.Characteristics);
				Loadbyte(ref _ExportDirectory.TimeDateStamp);
				Loadbyte(ref _ExportDirectory.MajorVersion);
				Loadbyte(ref _ExportDirectory.MinorVersion);
				Loadbyte(ref _ExportDirectory.Name);
				Loadbyte(ref _ExportDirectory.Base);
				Loadbyte(ref _ExportDirectory.NumberOfFunctions);
				Loadbyte(ref _ExportDirectory.NumberOfNames);
				Loadbyte(ref _ExportDirectory.AddressOfFunctions);
				Loadbyte(ref _ExportDirectory.AddressOfNames);
				Loadbyte(ref _ExportDirectory.AddressOfNameOrdinals);
				PEFileIndex = GetLong(_ExportDirectory.AddressOfFunctions) - GetLong(sectionData.SizeOfRawDataRVA) + GetLong(sectionData.PointerToRawData);
				long num = GetLong(_ExportDirectory.AddressOfNames) - GetLong(sectionData.SizeOfRawDataRVA) + GetLong(sectionData.PointerToRawData);
				long num2 = (num - PEFileIndex) / 4;
				for (long num3 = 0L; num3 != num2; num3++)
				{
					byte[] Data = new byte[4];
					Loadbyte(ref Data);
					_ExportDirectory.AddressOfFunctionsList.Add(Data);
				}
				num2 = 0L;
				PEFileIndex = num;
				num = GetLong(_ExportDirectory.AddressOfNameOrdinals) - GetLong(sectionData.SizeOfRawDataRVA) + GetLong(sectionData.PointerToRawData);
				num2 = (num - PEFileIndex) / 4;
				for (long num4 = 0L; num4 != num2; num4++)
				{
					byte[] Data2 = new byte[4];
					Loadbyte(ref Data2);
					_ExportDirectory.AddressOfNamesList.Add(Data2);
				}
				num2 = 0L;
				PEFileIndex = num;
				num = GetLong(_ExportDirectory.Name) - GetLong(sectionData.SizeOfRawDataRVA) + GetLong(sectionData.PointerToRawData);
				num2 = (num - PEFileIndex) / 2;
				for (long num5 = 0L; num5 != num2; num5++)
				{
					byte[] Data3 = new byte[2];
					Loadbyte(ref Data3);
					_ExportDirectory.AddressOfNameOrdinalsList.Add(Data3);
				}
				PEFileIndex = num;
				long num6 = 0L;
				while (true)
				{
					if (PEFileByte[PEFileIndex + num6] == 0)
					{
						if (PEFileByte[PEFileIndex + num6 + 1] == 0)
						{
							break;
						}
						byte[] Data4 = new byte[num6];
						Loadbyte(ref Data4);
						_ExportDirectory.NameList.Add(Data4);
						PEFileIndex++;
						num6 = 0L;
					}
					num6++;
				}
				break;
			}
		}

		private void LoadImportDirectory()
		{
			if (_OptionalDirAttrib.DirByte.Count < 1)
			{
				return;
			}
			OptionalDirAttrib.DirAttrib dirAttrib = (OptionalDirAttrib.DirAttrib)_OptionalDirAttrib.DirByte[1];
			long @long = GetLong(dirAttrib.DirRva);
			if (@long == 0L)
			{
				return;
			}
			long long2 = GetLong(dirAttrib.DirSize);
			_ImportDirectory = new ImportDirectory();
			long num = 0L;
			long num2 = 0L;
			long num3 = 0L;
			long num4 = 0L;
			for (int i = 0; i != _SectionTable.Section.Count; i++)
			{
				SectionTable.SectionData sectionData = (SectionTable.SectionData)_SectionTable.Section[i];
				num3 = GetLong(sectionData.SizeOfRawDataRVA);
				num4 = GetLong(sectionData.SizeOfRawDataSize);
				if (@long >= num3 && @long < num3 + num4)
				{
					num = GetLong(sectionData.SizeOfRawDataRVA);
					num2 = GetLong(sectionData.PointerToRawData);
					PEFileIndex = @long - num + num2;
					_ImportDirectory.FileStarIndex = PEFileIndex;
					_ImportDirectory.FileEndIndex = PEFileIndex + long2;
					break;
				}
			}
			if (num == 0L && num2 == 0L)
			{
				return;
			}
			while (true)
			{
				ImportDirectory.ImportDate importDate = new ImportDirectory.ImportDate();
				Loadbyte(ref importDate.OriginalFirstThunk);
				Loadbyte(ref importDate.TimeDateStamp);
				Loadbyte(ref importDate.ForwarderChain);
				Loadbyte(ref importDate.Name);
				Loadbyte(ref importDate.FirstThunk);
				if (GetLong(importDate.Name) == 0L)
				{
					break;
				}
				_ImportDirectory.ImportList.Add(importDate);
			}
			for (int j = 0; j != _ImportDirectory.ImportList.Count; j++)
			{
				ImportDirectory.ImportDate importDate2 = (ImportDirectory.ImportDate)_ImportDirectory.ImportList[j];
				long num5 = (PEFileIndex = GetLong(importDate2.Name) - num + num2);
				long num6;
				for (num6 = 0L; PEFileByte[PEFileIndex + num6] != 0; num6++)
				{
				}
				importDate2.DLLName = new byte[num6];
				Loadbyte(ref importDate2.DLLName);
			}
			for (int k = 0; k != _ImportDirectory.ImportList.Count; k++)
			{
				ImportDirectory.ImportDate importDate3 = (ImportDirectory.ImportDate)_ImportDirectory.ImportList[k];
				long num7 = (PEFileIndex = GetLong(importDate3.OriginalFirstThunk) - num + num2);
				while (true)
				{
					ImportDirectory.ImportDate.FunctionList functionList = new ImportDirectory.ImportDate.FunctionList();
					Loadbyte(ref functionList.OriginalFirst);
					long long3 = GetLong(functionList.OriginalFirst);
					if (long3 == 0L)
					{
						break;
					}
					long pEFileIndex = PEFileIndex;
					PEFileIndex = long3 - num + num2;
					if (long3 >= num3 && long3 < num3 + num4)
					{
						int num8 = 0;
						while (true)
						{
							if (num8 == 0)
							{
								Loadbyte(ref functionList.FunctionHead);
							}
							if (PEFileByte[PEFileIndex + num8] == 0)
							{
								break;
							}
							num8++;
						}
						byte[] Data = new byte[num8];
						Loadbyte(ref Data);
						functionList.FunctionName = Data;
					}
					else
					{
						functionList.FunctionName = new byte[1];
					}
					PEFileIndex = pEFileIndex;
					importDate3.DllFunctionList.Add(functionList);
				}
			}
		}

		private void LoadResourceDirectory()
		{
			if (_OptionalDirAttrib.DirByte.Count < 3)
			{
				return;
			}
			OptionalDirAttrib.DirAttrib dirAttrib = (OptionalDirAttrib.DirAttrib)_OptionalDirAttrib.DirByte[2];
			long @long = GetLong(dirAttrib.DirRva);
			if (@long == 0L)
			{
				return;
			}
			long long2 = GetLong(dirAttrib.DirSize);
			_ResourceDirectory = new ResourceDirectory();
			long num = 0L;
			long num2 = 0L;
			long num3 = 0L;
			long num4 = 0L;
			long pEIndex = 0L;
			for (int i = 0; i != _SectionTable.Section.Count; i++)
			{
				SectionTable.SectionData sectionData = (SectionTable.SectionData)_SectionTable.Section[i];
				num3 = GetLong(sectionData.SizeOfRawDataRVA);
				num4 = GetLong(sectionData.SizeOfRawDataSize);
				if (@long >= num3 && @long < num3 + num4)
				{
					num = GetLong(sectionData.SizeOfRawDataRVA);
					num2 = GetLong(sectionData.PointerToRawData);
					PEFileIndex = @long - num + num2;
					pEIndex = PEFileIndex;
					_ResourceDirectory.FileStarIndex = PEFileIndex;
					_ResourceDirectory.FileEndIndex = PEFileIndex + long2;
					break;
				}
			}
			if (num != 0L || num2 != 0L)
			{
				AddResourceNode(_ResourceDirectory, pEIndex, 0L, num3);
			}
		}

		private void AddResourceNode(ResourceDirectory Node, long PEIndex, long RVA, long ResourSectRva)
		{
			PEFileIndex = PEIndex + RVA;
			Loadbyte(ref Node.Characteristics);
			Loadbyte(ref Node.TimeDateStamp);
			Loadbyte(ref Node.MajorVersion);
			Loadbyte(ref Node.MinorVersion);
			Loadbyte(ref Node.NumberOfNamedEntries);
			Loadbyte(ref Node.NumberOfIdEntries);
			long @long = GetLong(Node.NumberOfNamedEntries);
			for (int i = 0; i != @long; i++)
			{
				ResourceDirectory.DirectoryEntry directoryEntry = new ResourceDirectory.DirectoryEntry();
				Loadbyte(ref directoryEntry.Name);
				Loadbyte(ref directoryEntry.Id);
				byte[] array = new byte[2]
				{
					directoryEntry.Name[0],
					directoryEntry.Name[1]
				};
				long num = GetLong(array) + PEIndex;
				array[0] = PEFileByte[num];
				array[1] = PEFileByte[num + 1];
				long long2 = GetLong(array);
				Node.Name = new byte[long2 * 2];
				for (int j = 0; j != Node.Name.Length; j++)
				{
					Node.Name[j] = PEFileByte[num + 2 + j];
				}
				array[0] = directoryEntry.Id[2];
				array[1] = directoryEntry.Id[3];
				long pEFileIndex = PEFileIndex;
				if (GetLong(array) == 0L)
				{
					array[0] = directoryEntry.Id[0];
					array[1] = directoryEntry.Id[1];
					PEFileIndex = GetLong(array) + PEIndex;
					ResourceDirectory.DirectoryEntry.DataEntry dataEntry = new ResourceDirectory.DirectoryEntry.DataEntry();
					Loadbyte(ref dataEntry.ResourRVA);
					Loadbyte(ref dataEntry.ResourSize);
					Loadbyte(ref dataEntry.ResourTest);
					Loadbyte(ref dataEntry.ResourWen);
					PEFileIndex = pEFileIndex;
					directoryEntry.DataEntryList.Add(dataEntry);
				}
				else
				{
					array[0] = directoryEntry.Id[0];
					array[1] = directoryEntry.Id[1];
					ResourceDirectory resourceDirectory = new ResourceDirectory();
					directoryEntry.NodeDirectoryList.Add(resourceDirectory);
					AddResourceNode(resourceDirectory, PEIndex, GetLong(array), ResourSectRva);
				}
				PEFileIndex = pEFileIndex;
				Node.EntryList.Add(directoryEntry);
			}
			long long3 = GetLong(Node.NumberOfIdEntries);
			for (int k = 0; k != long3; k++)
			{
				ResourceDirectory.DirectoryEntry directoryEntry2 = new ResourceDirectory.DirectoryEntry();
				Loadbyte(ref directoryEntry2.Name);
				Loadbyte(ref directoryEntry2.Id);
				byte[] array2 = new byte[2]
				{
					directoryEntry2.Id[2],
					directoryEntry2.Id[3]
				};
				long pEFileIndex2 = PEFileIndex;
				if (GetLong(array2) == 0L)
				{
					array2[0] = directoryEntry2.Id[0];
					array2[1] = directoryEntry2.Id[1];
					PEFileIndex = GetLong(array2) + PEIndex;
					ResourceDirectory.DirectoryEntry.DataEntry dataEntry2 = new ResourceDirectory.DirectoryEntry.DataEntry();
					Loadbyte(ref dataEntry2.ResourRVA);
					Loadbyte(ref dataEntry2.ResourSize);
					Loadbyte(ref dataEntry2.ResourTest);
					Loadbyte(ref dataEntry2.ResourWen);
					dataEntry2.FileEndIndex = (dataEntry2.FileStarIndex = GetLong(dataEntry2.ResourRVA) - ResourSectRva + PEIndex) + GetLong(dataEntry2.ResourSize);
					PEFileIndex = pEFileIndex2;
					directoryEntry2.DataEntryList.Add(dataEntry2);
				}
				else
				{
					array2[0] = directoryEntry2.Id[0];
					array2[1] = directoryEntry2.Id[1];
					ResourceDirectory resourceDirectory2 = new ResourceDirectory();
					directoryEntry2.NodeDirectoryList.Add(resourceDirectory2);
					AddResourceNode(resourceDirectory2, PEIndex, GetLong(array2), ResourSectRva);
				}
				PEFileIndex = pEFileIndex2;
				Node.EntryList.Add(directoryEntry2);
			}
		}

		private void Loadbyte(ref byte[] Data)
		{
			for (int i = 0; i != Data.Length; i++)
			{
				Data[i] = PEFileByte[PEFileIndex];
				PEFileIndex++;
			}
		}

		private string GetString(byte[] Data)
		{
			string text = "";
			for (int i = 0; i != Data.Length - 1; i++)
			{
				text = text + Data[i].ToString("X02") + " ";
			}
			return text + Data[Data.Length - 1].ToString("X02");
		}

		private string GetString(byte[] Data, string Type)
		{
			if (Type.Trim().ToUpper() == "ASCII")
			{
				return Encoding.ASCII.GetString(Data);
			}
			if (Type.Trim().ToUpper() == "DEFAULT")
			{
				return Encoding.Default.GetString(Data);
			}
			if (Type.Trim().ToUpper() == "UNICODE")
			{
				return Encoding.Unicode.GetString(Data);
			}
			if (Type.Trim().ToUpper() == "BYTE")
			{
				string text = "";
				for (int num = Data.Length - 1; num != 0; num--)
				{
					text = text + Data[num].ToString("X02") + " ";
				}
				return text + Data[0].ToString("X02");
			}
			return GetInt(Data);
		}

		private string GetInt(byte[] Data)
		{
			string text = "";
			for (int i = 0; i != Data.Length - 1; i++)
			{
				int num = Data[i];
				text = text + num + " ";
			}
			int num2 = Data[Data.Length - 1];
			return text + num2;
		}

		private long GetLong(byte[] Data)
		{
			string text = "";
			if (Data.Length <= 4)
			{
				for (int num = Data.Length - 1; num != -1; num--)
				{
					text += Data[num].ToString("X02");
				}
				return Convert.ToInt64(text, 16);
			}
			return 0L;
		}

		private void AddTableRow(DataTable RefTable, byte[] Data, string Name, string Describe)
		{
			RefTable.Rows.Add(Name, Data.Length.ToString(), GetString(Data), GetLong(Data).ToString(), GetString(Data, "ASCII"), Describe);
		}

		public DataSet GetPETable()
		{
			if (!_OpenFile)
			{
				return null;
			}
			DataSet dataSet = new DataSet("PEFile");
			if (_DosHeader != null)
			{
				dataSet.Tables.Add(TableDosHeader());
			}
			if (_PEHeader != null)
			{
				dataSet.Tables.Add(TablePEHeader());
			}
			if (_OptionalHeader != null)
			{
				dataSet.Tables.Add(TableOptionalHeader());
			}
			if (_OptionalDirAttrib != null)
			{
				dataSet.Tables.Add(TableOptionalDirAttrib());
			}
			if (_SectionTable != null)
			{
				dataSet.Tables.Add(TableSectionData());
			}
			if (_ExportDirectory != null)
			{
				dataSet.Tables.Add(TableExportDirectory());
				dataSet.Tables.Add(TableExportFunction());
			}
			if (_ImportDirectory != null)
			{
				dataSet.Tables.Add(TableImportDirectory());
				dataSet.Tables.Add(TableImportFunction());
			}
			if (_ResourceDirectory != null)
			{
				dataSet.Tables.Add(TableResourceDirectory());
			}
			return dataSet;
		}

		private DataTable TableDosHeader()
		{
			DataTable dataTable = new DataTable("DosHeader FileStar{" + _DosHeader.FileStarIndex + "}FileEnd{" + _DosHeader.FileEndIndex + "}");
			dataTable.Columns.Add("Name");
			dataTable.Columns.Add("Size");
			dataTable.Columns.Add("Value16");
			dataTable.Columns.Add("Value10");
			dataTable.Columns.Add("ASCII");
			dataTable.Columns.Add("Describe");
			AddTableRow(dataTable, _DosHeader.e_magic, "e_magic", "魔术数字");
			AddTableRow(dataTable, _DosHeader.e_cblp, "e_cblp", "文件最后页的字节数");
			AddTableRow(dataTable, _DosHeader.e_cp, "e_cp", "文件页数");
			AddTableRow(dataTable, _DosHeader.e_crlc, "e_crlc", "重定义元素个数");
			AddTableRow(dataTable, _DosHeader.e_cparhdr, "e_cparhdr", "头部尺寸，以段落为单位");
			AddTableRow(dataTable, _DosHeader.e_minalloc, "e_minalloc", "所需的最小附加段");
			AddTableRow(dataTable, _DosHeader.e_maxalloc, "e_maxalloc", "所需的最大附加段");
			AddTableRow(dataTable, _DosHeader.e_ss, "e_ss", "初始的SS值（相对偏移量）");
			AddTableRow(dataTable, _DosHeader.e_sp, "e_sp", "初始的SP值");
			AddTableRow(dataTable, _DosHeader.e_csum, "e_csum", "校验和");
			AddTableRow(dataTable, _DosHeader.e_ip, "e_ip", "初始的IP值");
			AddTableRow(dataTable, _DosHeader.e_cs, "e_cs", "初始的CS值（相对偏移量）");
			AddTableRow(dataTable, _DosHeader.e_rva, "e_rva", "");
			AddTableRow(dataTable, _DosHeader.e_fg, "e_fg", "");
			AddTableRow(dataTable, _DosHeader.e_bl1, "e_bl1", "");
			AddTableRow(dataTable, _DosHeader.e_oemid, "e_oemid", "");
			AddTableRow(dataTable, _DosHeader.e_oeminfo, "e_oeminfo", "");
			AddTableRow(dataTable, _DosHeader.e_bl2, "e_bl2", "");
			AddTableRow(dataTable, _DosHeader.e_PESTAR, "e_PESTAR", "PE开始 +本结构的位置");
			return dataTable;
		}

		private DataTable TablePEHeader()
		{
			DataTable dataTable = new DataTable("PeHeader FileStar{" + _PEHeader.FileStarIndex + "}FileEnd{" + _PEHeader.FileEndIndex + "}");
			dataTable.Columns.Add("Name");
			dataTable.Columns.Add("Size");
			dataTable.Columns.Add("Value16");
			dataTable.Columns.Add("Value10");
			dataTable.Columns.Add("ASCII");
			dataTable.Columns.Add("Describe");
			AddTableRow(dataTable, _PEHeader.Header, "Header", "PE文件标记");
			AddTableRow(dataTable, _PEHeader.Machine, "Machine", "该文件运行所要求的CPU。对于Intel平台，该值是IMAGE_FILE_MACHINE_I386 (14Ch)。我们尝试了LUEVELSMEYER的pe.txt声明的14Dh和14Eh，但Windows不能正确执行。 ");
			AddTableRow(dataTable, _PEHeader.NumberOfSections, "NumberOfSections", "文件的节数目。如果我们要在文件中增加或删除一个节，就需要修改这个值。");
			AddTableRow(dataTable, _PEHeader.TimeDateStamp, "TimeDateStamp", "文件创建日期和时间。 ");
			AddTableRow(dataTable, _PEHeader.PointerToSymbolTable, "PointerToSymbolTable", "用于调试。 ");
			AddTableRow(dataTable, _PEHeader.NumberOfSymbols, "NumberOfSymbols", "用于调试。 ");
			AddTableRow(dataTable, _PEHeader.SizeOfOptionalHeader, "SizeOfOptionalHeader", "指示紧随本结构之后的 OptionalHeader 结构大小，必须为有效值。");
			AddTableRow(dataTable, _PEHeader.Characteristics, "Characteristics", "关于文件信息的标记，比如文件是exe还是dll。");
			return dataTable;
		}

		private DataTable TableOptionalHeader()
		{
			DataTable dataTable = new DataTable("OptionalHeader FileStar{" + _OptionalHeader.FileStarIndex + "}FileEnd{" + _OptionalHeader.FileEndIndex + "}");
			dataTable.Columns.Add("Name");
			dataTable.Columns.Add("Size");
			dataTable.Columns.Add("Value16");
			dataTable.Columns.Add("Value10");
			dataTable.Columns.Add("ASCII");
			dataTable.Columns.Add("Describe");
			AddTableRow(dataTable, _OptionalHeader.Magic, "Magic", "Magic 010B=普通可以执行，0107=ROM映像");
			AddTableRow(dataTable, _OptionalHeader.MajorLinkerVersion, "MajorLinkerVersion", "主版本号");
			AddTableRow(dataTable, _OptionalHeader.MinorLinkerVersion, "MinorLinkerVersion", "副版本号");
			AddTableRow(dataTable, _OptionalHeader.SizeOfCode, "SizeOfCode", "代码段大小");
			AddTableRow(dataTable, _OptionalHeader.SizeOfInitializedData, "SizeOfInitializedData", "已初始化数据大小");
			AddTableRow(dataTable, _OptionalHeader.SizeOfUninitializedData, "SizeOfUninitializedData", "未初始化数据大小");
			AddTableRow(dataTable, _OptionalHeader.AddressOfEntryPoint, "AddressOfEntryPoint", "执行将从这里开始（RVA）");
			AddTableRow(dataTable, _OptionalHeader.BaseOfCode, "BaseOfCode", "代码基址（RVA）");
			AddTableRow(dataTable, _OptionalHeader.ImageBase, "ImageBase", "数据基址（RVA）");
			AddTableRow(dataTable, _OptionalHeader.ImageFileCode, "ImageFileCode", "映象文件基址");
			AddTableRow(dataTable, _OptionalHeader.SectionAlign, "SectionAlign", "区段列队");
			AddTableRow(dataTable, _OptionalHeader.MajorOSV, "MajorOSV", "文件列队");
			AddTableRow(dataTable, _OptionalHeader.MinorOSV, "MinorOSV", "操作系统主版本号");
			AddTableRow(dataTable, _OptionalHeader.MajorImageVer, "MajorImageVer", "映象文件主版本号");
			AddTableRow(dataTable, _OptionalHeader.MinorImageVer, "MinorImageVer", "映象文件副版本号");
			AddTableRow(dataTable, _OptionalHeader.MajorSV, "MajorSV", "子操作系统主版本号");
			AddTableRow(dataTable, _OptionalHeader.MinorSV, "MinorSV", "子操作系统副版本号");
			AddTableRow(dataTable, _OptionalHeader.UNKNOW, "UNKNOW", "Win32版本值");
			AddTableRow(dataTable, _OptionalHeader.SizeOfImage, "SizeOfImage", "映象文件大小");
			AddTableRow(dataTable, _OptionalHeader.SizeOfHeards, "SizeOfHeards", "标志头大小");
			AddTableRow(dataTable, _OptionalHeader.CheckSum, "CheckSum", "文件效验");
			AddTableRow(dataTable, _OptionalHeader.Subsystem, "Subsystem", "子系统（映象文件）1本地 2WINDOWS-GUI 3WINDOWS-CUI 4 POSIX-CUI");
			AddTableRow(dataTable, _OptionalHeader.DLL_Characteristics, "DLL_Characteristics", "DLL标记");
			AddTableRow(dataTable, _OptionalHeader.Bsize, "Bsize", "保留栈的大小");
			AddTableRow(dataTable, _OptionalHeader.TimeBsize, "TimeBsize", "初始时指定栈大小");
			AddTableRow(dataTable, _OptionalHeader.AucBsize, "AucBsize", "保留堆的大小");
			AddTableRow(dataTable, _OptionalHeader.SizeOfBsize, "SizeOfBsize", "初始时指定堆大小");
			AddTableRow(dataTable, _OptionalHeader.FuckBsize, "FuckBsize", "加载器标志");
			AddTableRow(dataTable, _OptionalHeader.DirectCount, "DirectCount", "数据目录数");
			return dataTable;
		}

		private DataTable TableOptionalDirAttrib()
		{
			DataTable dataTable = new DataTable("OptionalDirAttrib  FileStar{" + _OptionalDirAttrib.FileStarIndex + "}FileEnd{" + _OptionalDirAttrib.FileEndIndex + "}");
			dataTable.Columns.Add("Name");
			dataTable.Columns.Add("Size");
			dataTable.Columns.Add("Value16");
			dataTable.Columns.Add("Value10");
			dataTable.Columns.Add("ASCII");
			dataTable.Columns.Add("Describe");
			Hashtable hashtable = new Hashtable();
			hashtable.Add("0", "输出表");
			hashtable.Add("1", "输入表");
			hashtable.Add("2", "资源表");
			hashtable.Add("3", "异常表");
			hashtable.Add("4", "安全表");
			hashtable.Add("5", "基部重定位表");
			hashtable.Add("6", "调试数据");
			hashtable.Add("7", "版权数据");
			hashtable.Add("8", "全局PTR");
			hashtable.Add("9", "TLS表");
			hashtable.Add("10", "装入配置表");
			hashtable.Add("11", "其他表1");
			hashtable.Add("12", "其他表2");
			hashtable.Add("13", "其他表3");
			hashtable.Add("14", "其他表4");
			hashtable.Add("15", "其他表5");
			for (int i = 0; i != _OptionalDirAttrib.DirByte.Count; i++)
			{
				OptionalDirAttrib.DirAttrib dirAttrib = (OptionalDirAttrib.DirAttrib)_OptionalDirAttrib.DirByte[i];
				string name = "未知表";
				if (hashtable[i.ToString()] != null)
				{
					name = hashtable[i.ToString()].ToString();
				}
				AddTableRow(dataTable, dirAttrib.DirRva, name, "地址");
				AddTableRow(dataTable, dirAttrib.DirSize, "", "大小");
			}
			return dataTable;
		}

		private DataTable TableSectionData()
		{
			DataTable dataTable = new DataTable("SectionData FileStar{" + _SectionTable.FileStarIndex + "}FileEnd{" + _SectionTable.FileEndIndex + "}");
			dataTable.Columns.Add("Name");
			dataTable.Columns.Add("Size");
			dataTable.Columns.Add("Value16");
			dataTable.Columns.Add("Value10");
			dataTable.Columns.Add("ASCII");
			dataTable.Columns.Add("Describe");
			for (int i = 0; i != _SectionTable.Section.Count; i++)
			{
				SectionTable.SectionData sectionData = (SectionTable.SectionData)_SectionTable.Section[i];
				AddTableRow(dataTable, sectionData.SectName, "SectName", "名字");
				AddTableRow(dataTable, sectionData.VirtualAddress, "VirtualAddress", "虚拟内存地址");
				AddTableRow(dataTable, sectionData.SizeOfRawDataRVA, "SizeOfRawDataRVA", "RVA偏移");
				AddTableRow(dataTable, sectionData.SizeOfRawDataSize, "SizeOfRawDataSize", "RVA大小");
				AddTableRow(dataTable, sectionData.PointerToRawData, "PointerToRawData", "指向RAW数据");
				AddTableRow(dataTable, sectionData.PointerToRelocations, "PointerToRelocations", "指向定位号");
				AddTableRow(dataTable, sectionData.PointerToLinenumbers, "PointerToLinenumbers", "指向行数");
				AddTableRow(dataTable, sectionData.NumberOfRelocations, "NumberOfRelocations", "定位号");
				AddTableRow(dataTable, sectionData.NumberOfLinenumbers, "NumberOfLinenumbers", "行数号");
				AddTableRow(dataTable, sectionData.Characteristics, "Characteristics", "区段标记");
			}
			return dataTable;
		}

		private DataTable TableExportDirectory()
		{
			DataTable dataTable = new DataTable("ExportDirectory FileStar{" + _ExportDirectory.FileStarIndex + "}FileEnd{" + _ExportDirectory.FileEndIndex + "}");
			dataTable.Columns.Add("Name");
			dataTable.Columns.Add("Size");
			dataTable.Columns.Add("Value16");
			dataTable.Columns.Add("Value10");
			dataTable.Columns.Add("ASCII");
			dataTable.Columns.Add("Describe");
			AddTableRow(dataTable, _ExportDirectory.Characteristics, "Characteristics", "一个保留字段，目前为止值为0。");
			AddTableRow(dataTable, _ExportDirectory.TimeDateStamp, "TimeDateStamp", "产生的时间。");
			AddTableRow(dataTable, _ExportDirectory.MajorVersion, "MajorVersion", "主版本号");
			AddTableRow(dataTable, _ExportDirectory.MinorVersion, "MinorVersion", "副版本号");
			AddTableRow(dataTable, _ExportDirectory.Name, "Name", "一个RVA，指向一个dll的名称的ascii字符串。");
			AddTableRow(dataTable, _ExportDirectory.Base, "Base", "输出函数的起始序号。一般为1。");
			AddTableRow(dataTable, _ExportDirectory.NumberOfFunctions, "NumberOfFunctions", "输出函数入口地址的数组 中的元素个数。");
			AddTableRow(dataTable, _ExportDirectory.NumberOfNames, "NumberOfNames", "输出函数名的指针的数组 中的元素个数，也是输出函数名对应的序号的数组 中的元素个数。");
			AddTableRow(dataTable, _ExportDirectory.AddressOfFunctions, "AddressOfFunctions", "一个RVA，指向输出函数入口地址的数组。");
			AddTableRow(dataTable, _ExportDirectory.AddressOfNames, "AddressOfNames", "一个RVA，指向输出函数名的指针的数组。");
			AddTableRow(dataTable, _ExportDirectory.AddressOfNameOrdinals, "AddressOfNameOrdinals", "一个RVA，指向输出函数名对应的序号的数组。");
			return dataTable;
		}

		private DataTable TableExportFunction()
		{
			DataTable dataTable = new DataTable("ExportFunctionList");
			dataTable.Columns.Add("Name");
			dataTable.Columns.Add("Size");
			dataTable.Columns.Add("Value16");
			dataTable.Columns.Add("Value10");
			dataTable.Columns.Add("ASCII");
			dataTable.Columns.Add("Describe");
			for (int i = 0; i != _ExportDirectory.NameList.Count; i++)
			{
				AddTableRow(dataTable, (byte[])_ExportDirectory.NameList[i], "Name", "_ExportDirectory.Name-Sect.SizeOfRawDataRVA+Sect.PointerToRawData");
			}
			for (int j = 0; j != _ExportDirectory.AddressOfNamesList.Count; j++)
			{
				AddTableRow(dataTable, (byte[])_ExportDirectory.AddressOfNamesList[j], "NamesList", "");
			}
			for (int k = 0; k != _ExportDirectory.AddressOfFunctionsList.Count; k++)
			{
				AddTableRow(dataTable, (byte[])_ExportDirectory.AddressOfFunctionsList[k], "Functions", "");
			}
			for (int l = 0; l != _ExportDirectory.AddressOfNameOrdinalsList.Count; l++)
			{
				AddTableRow(dataTable, (byte[])_ExportDirectory.AddressOfNameOrdinalsList[l], "NameOrdinals", "");
			}
			return dataTable;
		}

		private DataTable TableImportDirectory()
		{
			DataTable dataTable = new DataTable("ImportDirectory FileStar{" + _ImportDirectory.FileStarIndex + "}FileEnd{" + _ImportDirectory.FileEndIndex + "}");
			dataTable.Columns.Add("Name");
			dataTable.Columns.Add("Size");
			dataTable.Columns.Add("Value16");
			dataTable.Columns.Add("Value10");
			dataTable.Columns.Add("ASCII");
			dataTable.Columns.Add("Describe");
			for (int i = 0; i != _ImportDirectory.ImportList.Count; i++)
			{
				ImportDirectory.ImportDate importDate = (ImportDirectory.ImportDate)_ImportDirectory.ImportList[i];
				AddTableRow(dataTable, importDate.DLLName, "输入DLL名称", "**********");
				AddTableRow(dataTable, importDate.OriginalFirstThunk, "OriginalFirstThunk", "这里实际上保存着一个RVA，这个RVA指向一个DWORD数组，这个数组可以叫做输入查询表。每个数组元素，或者叫一个表项，保存着一个指向函数名的RVA或者保存着一个函数的序号。");
				AddTableRow(dataTable, importDate.TimeDateStamp, "TimeDateStamp", "当这个值为0的时候，表明还没有bind。不为0的话，表示已经bind过了。有关bind的内容后面介绍。");
				AddTableRow(dataTable, importDate.ForwarderChain, "ForwarderChain", "");
				AddTableRow(dataTable, importDate.Name, "Name", "一个RVA，这个RVA指向一个ascii以空字符结束的字符串，这个字符串就是本结构对应的dll文件的名字。");
				AddTableRow(dataTable, importDate.FirstThunk, "FirstThunk", "一个RVA,这个RVA指向一个DWORD数组，这个数组可以叫输入地址表。如果bind了的话，这个数组的每个元素，就是一个输入函数的入口地址。");
			}
			return dataTable;
		}

		private DataTable TableImportFunction()
		{
			DataTable dataTable = new DataTable("ImportFunctionList");
			dataTable.Columns.Add("Name");
			dataTable.Columns.Add("Size");
			dataTable.Columns.Add("Value16");
			dataTable.Columns.Add("Value10");
			dataTable.Columns.Add("ASCII");
			dataTable.Columns.Add("Describe");
			for (int i = 0; i != _ImportDirectory.ImportList.Count; i++)
			{
				ImportDirectory.ImportDate importDate = (ImportDirectory.ImportDate)_ImportDirectory.ImportList[i];
				AddTableRow(dataTable, importDate.DLLName, "DLL-Name", "**********");
				for (int j = 0; j != importDate.DllFunctionList.Count; j++)
				{
					ImportDirectory.ImportDate.FunctionList functionList = (ImportDirectory.ImportDate.FunctionList)importDate.DllFunctionList[j];
					AddTableRow(dataTable, functionList.FunctionName, "FunctionName", "");
					AddTableRow(dataTable, functionList.FunctionHead, "FunctionHead", "");
					AddTableRow(dataTable, functionList.OriginalFirst, "OriginalFirstThunk", "");
				}
			}
			return dataTable;
		}

		private DataTable TableResourceDirectory()
		{
			DataTable dataTable = new DataTable("ResourceDirectory FileStar{" + _ResourceDirectory.FileStarIndex + "}FileEnd{" + _ResourceDirectory.FileEndIndex + "}");
			dataTable.Columns.Add("GUID");
			dataTable.Columns.Add("Text");
			dataTable.Columns.Add("ParentID");
			AddResourceDirectoryRow(dataTable, _ResourceDirectory, "");
			return dataTable;
		}

		private void AddResourceDirectoryRow(DataTable MyTable, ResourceDirectory Node, string ParentID)
		{
			string text = "";
			if (Node.Name != null)
			{
				text = GetString(Node.Name, "UNICODE");
			}
			int num = 0;
			while (num != Node.EntryList.Count)
			{
				ResourceDirectory.DirectoryEntry directoryEntry = (ResourceDirectory.DirectoryEntry)Node.EntryList[num];
				long @long = GetLong(directoryEntry.Name);
				string text2 = Guid.NewGuid().ToString();
				string text3 = "ID{" + @long + "}";
				if (text.Length != 0)
				{
					text3 = text3 + "Name{" + text + "}";
				}
				if (ParentID.Length == 0)
				{
					long num2 = @long - 1;
					if ((ulong)num2 > 15uL)
					{
						goto IL_020d;
					}
					switch (num2)
					{
					case 0L:
						break;
					case 1L:
						goto IL_0108;
					case 2L:
						goto IL_011b;
					case 3L:
						goto IL_012e;
					case 4L:
						goto IL_0141;
					case 5L:
						goto IL_0154;
					case 6L:
						goto IL_0167;
					case 7L:
						goto IL_017a;
					case 8L:
						goto IL_018d;
					case 9L:
						goto IL_019d;
					case 10L:
						goto IL_01ad;
					case 11L:
						goto IL_01bd;
					case 12L:
						goto IL_01cd;
					case 13L:
						goto IL_01dd;
					case 14L:
						goto IL_01ed;
					case 15L:
						goto IL_01fd;
					default:
						goto IL_020d;
					}
					text3 += "Type{Cursor}";
				}
				goto IL_021b;
				IL_0141:
				text3 += "Type{Menu}";
				goto IL_021b;
				IL_011b:
				text3 += "Type{Icon}";
				goto IL_021b;
				IL_012e:
				text3 += "Type{Cursor}";
				goto IL_021b;
				IL_0108:
				text3 += "Type{Bitmap}";
				goto IL_021b;
				IL_021b:
				MyTable.Rows.Add(text2, text3, ParentID);
				for (int i = 0; i != directoryEntry.DataEntryList.Count; i++)
				{
					ResourceDirectory.DirectoryEntry.DataEntry dataEntry = (ResourceDirectory.DirectoryEntry.DataEntry)directoryEntry.DataEntryList[i];
					string text4 = "Address{" + GetString(dataEntry.ResourRVA) + "} Size{" + GetString(dataEntry.ResourSize) + "} FileBegin{" + dataEntry.FileStarIndex + "-" + dataEntry.FileEndIndex + "}";
					MyTable.Rows.Add(Guid.NewGuid().ToString(), text4, text2);
				}
				for (int j = 0; j != directoryEntry.NodeDirectoryList.Count; j++)
				{
					AddResourceDirectoryRow(MyTable, (ResourceDirectory)directoryEntry.NodeDirectoryList[j], text2);
				}
				num++;
				continue;
				IL_020d:
				text3 += "Type{未定义}";
				goto IL_021b;
				IL_01fd:
				text3 += "Type{Version}";
				goto IL_021b;
				IL_01ed:
				text3 += "Type{Information}";
				goto IL_021b;
				IL_01dd:
				text3 += "Type{Group Icon}";
				goto IL_021b;
				IL_01cd:
				text3 += "Type{Group Cursor}";
				goto IL_021b;
				IL_01bd:
				text3 += "Type{Message Table}";
				goto IL_021b;
				IL_01ad:
				text3 += "Type{Unformatted}";
				goto IL_021b;
				IL_019d:
				text3 += "Type{Accelerators}";
				goto IL_021b;
				IL_018d:
				text3 += "Type{Font}";
				goto IL_021b;
				IL_017a:
				text3 += "Type{Font Directory}";
				goto IL_021b;
				IL_0167:
				text3 += "Type{String Table}";
				goto IL_021b;
				IL_0154:
				text3 += "Type{Dialog}";
				goto IL_021b;
			}
		}
	}
}
