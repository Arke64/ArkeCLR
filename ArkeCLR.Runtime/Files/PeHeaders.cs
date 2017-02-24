using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ArkeCLR.Runtime.Files {
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

        public override string ToString() => Encoding.ASCII.GetString(this.NameData, 0, 8).Replace("\0", "");
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
}
