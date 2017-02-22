using ArkeCLR.Utilities;
using ArkeCLR.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ArkeCLR.Runtime.Headers {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RvaAndSize {
        public uint Rva;
        public uint Size;

        public bool IsZero => this.Rva == 0 && this.Size == 0;

        public override string ToString() => $"RVA: 0x{this.Rva:X8} Size: 0x{this.Size:X8}";
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DosHeader {
        public ushort MagicNumber;
        public ushort LastPageBytes;
        public ushort PagesInFile;
        public ushort Relocations;
        public ushort HeaderSizeInParagraphs;
        public ushort MinimumParagraphs;
        public ushort MaximumParagraphs;
        public ushort InitialSsValue;
        public ushort InitialSpValue;
        public ushort Checksum;
        public ushort InitialIpValue;
        public ushort InitialCsValue;
        public ushort RelocationTableAddress;
        public ushort OverlayNumber;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] Reserved1;

        public ushort OemId;
        public ushort OemInfo;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public ushort[] Reserved2;

        public uint NewHeaderAddress;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NtHeader {
        public uint Signature;
        public CoffHeader CoffHeader;
        public StandardFields StandardFields;
        public NtSpecificFields NtSpecificFields;

        public void Verify() {
            if (this.Signature != 0x00004550) throw new InvalidFileException(nameof(this.Signature));

            this.CoffHeader.Verify();
            this.StandardFields.Verify();
            this.NtSpecificFields.Verify();
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CoffHeader {
        public Machine Machine;
        public ushort NumberOfSections;
        public uint TimeDateStamp;
        public uint PointerToSymbolTable;
        public uint NumberOfSymbols;
        public ushort OptionalHeaderSize;
        public CoffCharacteristics Characteristics;

        public void Verify() {
            if (!this.Machine.IsValid()) throw new InvalidFileException(nameof(this.Machine));
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct StandardFields {
        public CoffMagicNumber MagicNumber;
        public byte MajorLinkerVersion;
        public byte MinorLinkerVersion;
        public uint CodeSize;
        public uint InitializedDataSize;
        public uint UninitializedDataSize;
        public uint EntryPointRva;
        public uint BaseOfCode;
        public uint BaseOfData;

        public void Verify() {
            if (this.MagicNumber != CoffMagicNumber.Pe32) throw new InvalidFileException("Only PE32 files supported.");
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NtSpecificFields {
        public uint ImageBase;
        public uint SectionAlignment;
        public uint FileAlignment;
        public ushort MajorOsVersion;
        public ushort MinorOsVersion;
        public ushort MajorUserVersion;
        public ushort MinorUserVersion;
        public ushort MajorSubsystemVersion;
        public ushort MinorSubsystemVersion;
        public uint Win32Version;
        public uint ImageSize;
        public uint HeaderSize;
        public uint FileChecksum;
        public NtSpecificSubSystem SubSystem;
        public DllCharacteristics DllFlags;
        public uint StackReserveSize;
        public uint StackCommitSize;
        public uint HeapReserveSize;
        public uint HeapCommitSize;
        public uint LoaderFlags;
        public uint NumberOfDataDirectories;

        public void Verify() {
            if (this.ImageBase % 0x10000 != 0) throw new InvalidFileException(nameof(this.ImageBase));
            if (this.SectionAlignment <= this.FileAlignment) throw new InvalidFileException(nameof(this.SectionAlignment));
            if (this.FileAlignment != 0x200) throw new InvalidFileException(nameof(this.FileAlignment));
            if (this.HeaderSize % this.FileAlignment != 0) throw new InvalidFileException(nameof(this.HeaderSize));
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
        public SectionCharacteristics Characteristics;

        public string Name => Encoding.ASCII.GetString(this.NameData, 0, 8).Replace("\0", "");

        public override string ToString() => this.Name;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CliHeader {
        public uint HeaderSize;
        public ushort MajorRuntimeVersion;
        public ushort MinorRuntimeVersion;
        public RvaAndSize Metadata;
        public CliRuntimeFlags Flags;
        public uint EntryPointToken;
        public RvaAndSize Resources;
        public RvaAndSize StrongNameSignature;
        public ulong CodeManagerTable;
        public RvaAndSize VTableFixups;
        public ulong ExportAddressTableJumps;
        public ulong ManagedNativeHeader;

        public void Verify() {
            if (this.Metadata.IsZero) throw new InvalidFileException(nameof(this.Metadata));
        }
    }

    public struct CilMetadataRootHeader : ICustomByteReader {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct RootHeader1 {
            public uint Signature;
            public ushort MajorVersion;
            public ushort MinorVersion;
            public uint Reserved1;
            public uint VersionLength;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct RootHeader2 {
            public ushort Flags;
            public ushort StreamCount;
        }

        public uint Signature;
        public ushort MajorVersion;
        public ushort MinorVersion;
        public uint Reserved1;
        public uint VersionLength;
        public string Version;
        public ushort Flags;
        public ushort StreamCount;

        public void Read(ByteReader file) {
            var root1 = file.ReadStruct<RootHeader1>();
            this.Signature = root1.Signature;
            this.MajorVersion = root1.MajorVersion;
            this.MinorVersion = root1.MinorVersion;
            this.Reserved1 = root1.Reserved1;
            this.VersionLength = root1.VersionLength;

            this.Version = file.ReadStringFixed(Encoding.UTF8, this.VersionLength, 0);

            var root2 = file.ReadStruct<RootHeader2>();
            this.Flags = root2.Flags;
            this.StreamCount = root2.StreamCount;
        }

        public void Verify() {
            if (this.Signature != 0x424A5342) throw new InvalidFileException(nameof(this.Signature));
        }
    }

    public struct CilMetadataStreamHeader : ICustomByteReader {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct StreamHeader1 {
            public uint Offset;
            public uint Size;
        }

        public uint Offset;
        public uint Size;
        public string Name;

        public void Read(ByteReader file) {
            var header1 = file.ReadStruct<StreamHeader1>();
            this.Offset = header1.Offset;
            this.Size = header1.Size;

            this.Name = file.ReadStringAligned(Encoding.ASCII, 0, 4);
        }

        public override string ToString() => this.Name;
    }

    public struct CilTableStreamHeader : ICustomByteReader {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct StreamHeader1 {
            public uint Reserved1;
            public byte MajorVersion;
            public byte MinorVersion;
            public byte HeapSizes;
            public byte Reserved2;
            public ulong Valid;
            public ulong Sorted;
        }

        public uint Reserved1;
        public byte MajorVersion;
        public byte MinorVersion;
        public BitVector HeapSizes;
        public byte Reserved2;
        public BitVector Valid;
        public BitVector Sorted;
        public uint[] Rows;

        public void Read(ByteReader file) {
            var header1 = file.ReadStruct<StreamHeader1>();
            this.Reserved1 = header1.Reserved1;
            this.MajorVersion = header1.MajorVersion;
            this.MinorVersion = header1.MinorVersion;
            this.HeapSizes = new BitVector(header1.HeapSizes);
            this.Reserved2 = header1.Reserved2;
            this.Valid = new BitVector(header1.Valid);
            this.Sorted = new BitVector(header1.Sorted);

            var rows = file.ReadArray<uint>(this.Valid.CountSet());
            var i = 0;

            this.Rows = this.Valid.Select(v => v ? rows[i++] : 0).ToArray();
        }
    }

    public enum Machine : ushort {
        Unknown = 0x0000,
        Am33 = 0x01D3,
        Amd64 = 0x8664,
        Arm = 0x01C0,
        Arm64 = 0xAA64,
        ArmNt = 0x01C4,
        Ebc = 0x0EBC,
        I386 = 0x014C,
        Ia64 = 0x0200,
        M32R = 0x9041,
        Mips16 = 0x0266,
        MipsFpu = 0x0366,
        MipsFpu16 = 0x0466,
        PowerPc = 0x01F0,
        PowerPcFp = 0x01F1,
        R4000 = 0x0166,
        RiscV32 = 0x5032,
        RiscV64 = 0x5064,
        RiscV128 = 0x5128,
        Sh3 = 0x01A2,
        Sh3Dsp = 0x01A3,
        Sh4 = 0x01A6,
        Sh5 = 0x01A8,
        Thumb = 0x01C2,
        WceMipsV2 = 0x0169,
    }

    public enum CoffMagicNumber : ushort {
        Rom = 0x107,
        Pe32 = 0x10B,
        Pe32Plus = 0x20B
    }

    public enum NtSpecificSubSystem : ushort {
        Unknown = 0x0,
        Native = 0x1,
        Gui = 0x2,
        Cui = 0x3,
        PosixCui = 0x7,
        WindowsCeGui = 0x9,
        EfiApplication = 0xA,
        EfiBootServiceDriver = 0xB,
        EfiRuntimeDriver = 0xC,
        EfiRom = 0xD,
        Xbox = 0xE
    }

    public enum DataDirectoryType {
        ExportTable = 0,
        ImportTable = 1,
        ResourceTable = 2,
        ExceptionTable = 3,
        CertificateTable = 4,
        BaseRelocationTable = 5,
        Debug = 6,
        Copyright = 7,
        GlobalPtr = 8,
        TlsTable = 9,
        LoadConfigTable = 10,
        BoundImport = 11,
        Iat = 12,
        DelayImportDescriptior = 13,
        CliHeader = 14
    }

    [Flags]
    public enum CoffCharacteristics : ushort {
        RelocsStripped = 0x0001,
        ExecutableImage = 0x0002,
        LineNumbersStripped = 0x0004,
        LocalSymbolsStripped = 0x0008,
        AggressiveWorkingSetTrim = 0x0010,
        LargeAddressAware = 0x0020,
        Reserved1 = 0x0040,
        BytesReversedLo = 0x0080,
        X86Machine = 0x0100,
        DebugStripped = 0x0200,
        RemovableRunFromSWap = 0x0400,
        NetRunFromSwap = 0x0800,
        FileSystem = 0x1000,
        Dll = 0x2000,
        UpSystemOnly = 0x4000,
        BytesReversedHigh = 0x8000
    }

    [Flags]
    public enum DllCharacteristics : ushort {
        Reserved1 = 0x0001,
        Reserved2 = 0x0002,
        Reserved3 = 0x0004,
        Reserved4 = 0x0008,
        Reserved5 = 0x0010,
        HighEntropyVa = 0x0020,
        DynamicBase = 0x0040,
        ForceIntegiry = 0x0080,
        NxCompatbility = 0x0100,
        NoIsolation = 0x0200,
        NoSeh = 0x0400,
        NoBind = 0x0800,
        AppContainer = 0x1000,
        WdmDriver = 0x2000,
        GuardCf = 0x4000,
        TerminalServerAware = 0x8000
    }

    [Flags]
    public enum SectionCharacteristics : uint {
        Reserved1 = 0x00000000,
        Reserved2 = 0x00000001,
        Reserved3 = 0x00000002,
        Reserved4 = 0x00000004,
        TypeNoPad = 0x00000008,
        Reserved5 = 0x00000010,
        ContainsCode = 0x00000020,
        ContainsInitializedData = 0x00000040,
        ContainsUninitializedData = 0x00000080,
        LinkOther = 0x00000100,
        LinkInfo = 0x00000200,
        Reserved6 = 0x00000400,
        LinkRemove = 0x00000800,
        LinkComdat = 0x00001000,
        GPREL = 0x00008000,
        MemoryPurgable = 0x00020000,
        Memory16Bit = 0x00020000,
        MemoryLocked = 0x00040000,
        MemoryPreloaded = 0x00080000,
        Align1Bytes = 0x00100000,
        Align2Bytes = 0x00200000,
        Align4Bytes = 0x00300000,
        Align8Bytes = 0x00400000,
        Align16Bytes = 0x00500000,
        Align32Bytes = 0x00600000,
        Align64Bytes = 0x00700000,
        Align128Bytes = 0x00800000,
        Align256Bytes = 0x00900000,
        Align512Bytes = 0x00A00000,
        Align1024Bytes = 0x00B00000,
        Align2048Bytes = 0x00C00000,
        Align4096Bytes = 0x00D00000,
        Align8192Bytes = 0x00E00000,
        LinkRelocOverflow = 0x01000000,
        MemoryDiscardable = 0x02000000,
        MemoryNotCached = 0x04000000,
        MemoryNotPaged = 0x08000000,
        MemoryShared = 0x10000000,
        MemoryExecute = 0x20000000,
        MemoryRead = 0x40000000,
        MemoryWrite = 0x80000000,
    }

    [Flags]
    public enum CliRuntimeFlags : uint {
        IlOnly = 0x00000001,
        X86Required = 0x00000002,
        StrongNameSigned = 0x00000008,
        NativeEntryPointer = 0x00000010,
        TrackDebugData = 0x00010000,
        Prefer32Bit = 0x00020000,
    }
}
