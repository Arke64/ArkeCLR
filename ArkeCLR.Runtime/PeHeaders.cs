using ArkeCLR.Utilities.Extensions;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ArkeCLR.Runtime.Pe {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RvaAndSize {
        public uint Rva;
        public uint Size;

        public bool IsZero => this.Rva == 0 && this.Size == 0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Header {
        private static byte[] MsDosStubData { get; } = new byte[] { 0x4D, 0x5A, 0x90, 0x00, 0x03, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x00, 0x00, 0xB8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x0E, 0x1F, 0xBA, 0x0E, 0x00, 0xB4, 0x09, 0xCD, 0x21, 0xB8, 0x01, 0x4C, 0xCD, 0x21, 0x54, 0x68, 0x69, 0x73, 0x20, 0x70, 0x72, 0x6F, 0x67, 0x72, 0x61, 0x6D, 0x20, 0x63, 0x61, 0x6E, 0x6E, 0x6F, 0x74, 0x20, 0x62, 0x65, 0x20, 0x72, 0x75, 0x6E, 0x20, 0x69, 0x6E, 0x20, 0x44, 0x4F, 0x53, 0x20, 0x6D, 0x6F, 0x64, 0x65, 0x2E, 0x0D, 0x0D, 0x0A, 0x24, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] MsDosStub;
        public uint Signature;
        public FileHeader File;
        public OptionalHeader Optional;

        public void Verify() {
            if (this.Signature != 0x00004550) throw new InvalidPeFileException(InvalidPeFilePart.Signature);

            for (var i = 0; i < Header.MsDosStubData.Length; i++)
                if (this.MsDosStub[i] != Header.MsDosStubData[i])
                    throw new InvalidPeFileException(InvalidPeFilePart.MsDosStub);

            this.File.Verify();
            this.Optional.Verify();
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FileHeader {
        public Machine Machine;
        public ushort NumberOfSections;
        public uint TimeStamp;
        public uint PointerToSymbolTable;
        public uint NumberOfSymbols;
        public ushort OptionalHeaderSize;
        public FileHeaderCharacteristics Characteristics;

        public void Verify() {
            if (!this.Machine.IsValid() || this.PointerToSymbolTable != 0 || this.NumberOfSymbols != 0 || this.OptionalHeaderSize != 0xE0) throw new InvalidPeFileException(InvalidPeFilePart.FileHeader);
            if (!this.Characteristics.HasFlag(FileHeaderCharacteristics.ExecutableImage) || this.Characteristics.HasFlag(FileHeaderCharacteristics.RelocsStripped)) throw new InvalidPeFileException(InvalidPeFilePart.FileHeader);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct OptionalHeader {
        public StandardFields StandardFields;
        public NtSpecificFields NtSpecificFields;
        public DataDirectories DataDirectories;

        public void Verify() {
            this.StandardFields.Verify();
            this.NtSpecificFields.Verify();
            this.DataDirectories.Verify();
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct StandardFields {
        public ushort Magic;
        public byte LMajor;
        public byte LMinor;
        public uint CodeSize;
        public uint InitializedDataSize;
        public uint UninitializedDataSize;
        public uint EntryPointRva;
        public uint BaseOfCode;
        public uint BaseOfData;

        public void Verify() {
            if (this.Magic != 0x10B /*|| this.LMajor != 6 || this.LMinor != 0*/) throw new InvalidPeFileException(InvalidPeFilePart.StandardFields);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NtSpecificFields {
        public uint ImageBase;
        public uint SectionAlignment;
        public uint FileAlignment;
        public ushort OsMajor;
        public ushort OsMinor;
        public ushort UserMajor;
        public ushort UserMinor;
        public ushort SubSystemMajor;
        public ushort SubSystemMinor;
        public uint Reserved;
        public uint ImageSize;
        public uint HeaderSize;
        public uint FileChecksum;
        public NtSpecificSubSystem SubSystem;
        public ushort DllFlags;
        public uint StackReserveSize;
        public uint StackCommitSize;
        public uint HeapReserveSize;
        public uint HeapCommitSize;
        public uint LoaderFlags;
        public uint NumberOfDataDirectories;

        public void Verify() {
            if (this.ImageBase % 0x10000 != 0 || this.SectionAlignment <= this.FileAlignment || this.FileAlignment != 0x200 || this.HeaderSize % this.FileAlignment != 0) throw new InvalidPeFileException(InvalidPeFilePart.NtSpecificFields);
            if (/*this.OsMajor != 5 || this.OsMinor != 0 ||*/ this.UserMajor != 0 || this.UserMinor != 0 || /*this.SubSystemMajor != 5 || this.SubSystemMinor != 0 ||*/ this.Reserved != 0) throw new InvalidPeFileException(InvalidPeFilePart.NtSpecificFields);
            if (this.FileChecksum != 0 || (this.SubSystem != NtSpecificSubSystem.Gui && this.SubSystem != NtSpecificSubSystem.Cui) || (this.DllFlags & 0x100F) != 0 || this.LoaderFlags != 0 || this.NumberOfDataDirectories != 0x10) throw new InvalidPeFileException(InvalidPeFilePart.NtSpecificFields);
            if (this.StackReserveSize != 0x100000 || this.StackCommitSize != 0x1000 || this.HeapReserveSize != 0x100000 || this.HeapCommitSize != 0x1000) throw new InvalidPeFileException(InvalidPeFilePart.NtSpecificFields);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DataDirectories {
        public RvaAndSize ExportTable;
        public RvaAndSize ImportTable;
        public RvaAndSize ResourceTable;
        public RvaAndSize ExceptionTable;
        public RvaAndSize CertificateTable;
        public RvaAndSize BaseRelocationTable;
        public RvaAndSize Debug;
        public RvaAndSize Copyright;
        public RvaAndSize GlobalPtr;
        public RvaAndSize TlsTable;
        public RvaAndSize LoadConfigTable;
        public RvaAndSize BoundImport;
        public RvaAndSize Iat;
        public RvaAndSize DelayImportDescriptior;
        public RvaAndSize CliHeader;
        public RvaAndSize Reserved;

        public void Verify() {
            if (!this.ExportTable.IsZero || /*!this.ResourceTable.IsZero || */ !this.ExceptionTable.IsZero || !this.CertificateTable.IsZero || /*this.Debug.IsZero || */ !this.Copyright.IsZero || !this.GlobalPtr.IsZero || !this.TlsTable.IsZero || !this.LoadConfigTable.IsZero || !this.BoundImport.IsZero || !this.DelayImportDescriptior.IsZero || !this.Reserved.IsZero) throw new InvalidPeFileException(InvalidPeFilePart.DataDirectories);
            if (this.CliHeader.IsZero) throw new InvalidPeFileException(InvalidPeFilePart.DataDirectories);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SectionHeader {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] NameData;
        public uint VirtualSize;
        public uint VirtualAddress;
        public uint SizeOfRawData;
        public uint PointerToRawData;
        public uint PointerToRelocations;
        public uint PointerToLineNumbers;
        public ushort NumberOfRelocations;
        public ushort NumberOfLineNumbers;
        public SectionHeaderCharacteristics Characteristics;

        public string Name => Encoding.ASCII.GetString(this.NameData, 0, 8).Replace("\0", "");
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CliHeader {
        public uint Cb;
        public ushort MajorRuntimeVersion;
        public ushort MinorRuntimeVersion;
        public RvaAndSize Metadata;
        public CliRuntimeFlags Flags;
        public uint EntryPointToken;
        public RvaAndSize Resources;
        public ulong StrongNameSignature;
        public ulong CodeManagerTable;
        public ulong VTableFixups;
        public ulong ExportAddressTableJumps;
        public ulong ManagedNativeHeader;
    }

    public enum Machine : ushort {
        I386 = 0x014C,
        AMD64 = 0x8664,
        IA64 = 0x0200,
        ARMv7 = 0x01C4,
    }

    public enum NtSpecificSubSystem : ushort {
        Gui = 0x2,
        Cui = 0x3
    }

    [Flags]
    public enum FileHeaderCharacteristics : ushort {
        RelocsStripped = 0x0001,
        ExecutableImage = 0x0002,
        X86Machine = 0x0100,
        Dll = 0x2000
    }

    [Flags]
    public enum SectionHeaderCharacteristics : uint {
        ContainsCode = 0x00000020,
        ContainsInitializedData = 0x00000040,
        ContainsUninitializedData = 0x00000080,
        MemoryExecute = 0x20000000,
        MemoryRead = 0x40000000,
        MemoryWrite = 0x80000000
    }

    [Flags]
    public enum CliRuntimeFlags : uint {
        IlOnly = 0x00001,
        X86Required = 0x00002,
        StrongNameSIgned = 0x00008,
        NativeEntryPointer = 0x00010,
        TrackDebugData = 0x10000
    }
}
