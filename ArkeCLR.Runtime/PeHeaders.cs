﻿using System;
using System.Runtime.InteropServices;

namespace ArkeCLR.Runtime.Pe {
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
        public ushort Machine;
        public ushort NumberOfSections;
        public uint TimeStamp;
        public uint PointerToSymbolTable;
        public uint NumberOfSymbols;
        public ushort OptionalHeaderSize;
        public HeaderCharacteristics Characteristics;

        public void Verify() {
            if (this.Machine != 0x14C || this.PointerToSymbolTable != 0 || this.NumberOfSymbols != 0 || this.OptionalHeaderSize != 0xE0) throw new InvalidPeFileException(InvalidPeFilePart.FileHeader);
            if (!this.Characteristics.HasFlag(HeaderCharacteristics.ExecutableImage) || this.Characteristics.HasFlag(HeaderCharacteristics.RelocsStripped)) throw new InvalidPeFileException(InvalidPeFilePart.FileHeader);
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
        public SubSystem SubSystem;
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
            if (this.FileChecksum != 0 || (this.SubSystem != SubSystem.Gui && this.SubSystem != SubSystem.Cui) || (this.DllFlags & 0x100F) != 0 || this.LoaderFlags != 0 || this.NumberOfDataDirectories != 0x10) throw new InvalidPeFileException(InvalidPeFilePart.NtSpecificFields);
            if (this.StackReserveSize != 0x100000 || this.StackCommitSize != 0x1000 || this.HeapReserveSize != 0x100000 || this.HeapCommitSize != 0x1000) throw new InvalidPeFileException(InvalidPeFilePart.NtSpecificFields);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DataDirectories {
        public ulong ExportTable;
        public ulong ImportTable;
        public ulong ResourceTable;
        public ulong ExceptionTable;
        public ulong CertificateTable;
        public ulong BaseRelocationTable;
        public ulong Debug;
        public ulong Copyright;
        public ulong GlobalPtr;
        public ulong TlsTable;
        public ulong LoadConfigTable;
        public ulong BoundImport;
        public ulong Iat;
        public ulong DelayImportDescriptior;
        public ulong CliHeader;
        public ulong Reserved;

        public void Verify() {
            if (this.ExportTable != 0 || /*this.ResourceTable != 0 ||*/ this.ExceptionTable != 0 || this.CertificateTable != 0 || /*this.Debug != 0 ||*/ this.Copyright != 0 || this.GlobalPtr != 0 || this.TlsTable != 0 || this.LoadConfigTable != 0 || this.BoundImport != 0 || this.DelayImportDescriptior != 0 || this.Reserved != 0) throw new InvalidPeFileException(InvalidPeFilePart.DataDirectories);
        }
    }

    [Flags]
    public enum HeaderCharacteristics : ushort {
        RelocsStripped = 0x0001,
        ExecutableImage = 0x0002,
        x86Machine = 0x0100,
        Dll = 0x2000
    }

    public enum SubSystem : ushort {
        Gui = 0x2,
        Cui = 0x3
    }
}
