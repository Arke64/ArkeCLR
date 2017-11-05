using ArkeCLR.Utilities;
using ArkeCLR.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

#pragma warning disable S1104 // Fields should not have public accessibility

namespace ArkeCLR.Runtime.Files {
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

            this.Version = file.ReadString(Encoding.UTF8, this.VersionLength);

            var root2 = file.ReadStruct<RootHeader2>();
            this.Flags = root2.Flags;
            this.StreamCount = root2.StreamCount;
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

            this.Name = file.ReadStringTerminated(Encoding.ASCII, 0, 4);
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

    public struct MethodHeader : ICustomByteReader {
        public MethodFlags Flags;
        public byte Size;
        public ushort MaxStack;
        public uint CodeSize;
        public uint LocalVarSigTok;

        public byte[] Body;
        public MethodDataSectionHeader[] DataSections;

        public void Read(ByteReader file) {
            if (((MethodFlags)file.PeekU1()).FlagSet(MethodFlags.FatFormat)) {
                var first = file.ReadU2();

                this.Flags = (MethodFlags)((first >> 4) & 0b1111_1111_1111);
                this.Size = (byte)(first & 0b1111);

                file.Read(out this.MaxStack);
                file.Read(out this.CodeSize);
                file.Read(out this.LocalVarSigTok);
            }
            else {
                var first = file.ReadU1();

                this.Flags = (MethodFlags)(first & 0b11);
                this.CodeSize = (first & 0b1111_1100U) >> 2;
                this.MaxStack = 8;
            }

            this.Body = file.ReadArray<byte>(this.CodeSize);

            var sects = new List<MethodDataSectionHeader>();

            if (this.Flags.FlagSet(MethodFlags.MoreSects)) {
                file.SeekToMultiple(4);

                var sect = default(MethodDataSectionHeader);

                do {
                    sects.Add(sect = file.ReadCustom<MethodDataSectionHeader>());
                } while (sect.Kind.FlagSet(MethodDataSectionFlags.MoreSects));
            }

            this.DataSections = sects.ToArray();
        }
    }

    public struct MethodDataSectionHeader : ICustomByteReader {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct TinyExceptionHandlingClause {
            public ushort Flags;
            public ushort TryOffset;
            public byte TryLength;
            public ushort HandlerOffset;
            public byte HandlerLength;
            public uint ClassToken;
            public uint FilterOffset;

            public ExceptionHandlingClause Expand() => new ExceptionHandlingClause {
                Flags = (ExceptionClauseFlags)this.Flags,
                TryOffset = this.TryOffset,
                TryLength = this.TryLength,
                HandlerOffset = this.HandlerOffset,
                HandlerLength = this.HandlerLength,
                ClassToken = this.ClassToken,
                FilterOffset = this.FilterOffset,
            };
        }

        public MethodDataSectionFlags Kind;
        public uint DataSize;
        public ExceptionHandlingClause[] Clauses;

        public void Read(ByteReader file) {
            file.ReadEnum(out this.Kind);
            file.Read(out this.DataSize);

            if (!this.Kind.FlagSet(MethodDataSectionFlags.FatFormat)) {
                this.DataSize >>= 16;

                this.Clauses = file.ReadArray<TinyExceptionHandlingClause>((this.DataSize - 4) / 12).ToArray(c => c.Expand());
            }
            else {
                this.Clauses = file.ReadArray<ExceptionHandlingClause>((this.DataSize - 4) / 24);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ExceptionHandlingClause {
        public ExceptionClauseFlags Flags;
        public uint TryOffset;
        public uint TryLength;
        public uint HandlerOffset;
        public uint HandlerLength;
        public uint ClassToken;
        public uint FilterOffset;
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

    [Flags]
    public enum MethodFlags : ushort {
        TinyFormat = 0x02,
        FatFormat = 0x03,
        MoreSects = 0x08,
        InitLocals = 0x10
    }

    [Flags]
    public enum MethodDataSectionFlags : uint {
        EHTable = 0x01,
        OptILTable = 0x02,
        FatFormat = 0x40,
        MoreSects = 0x80
    }

    [Flags]
    public enum ExceptionClauseFlags : uint {
        Exception = 0x00,
        Filter = 0x01,
        Finally = 0x02,
        Fault = 0x04
    }
}
