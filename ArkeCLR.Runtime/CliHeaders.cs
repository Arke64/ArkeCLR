using ArkeCLR.Utilities;
using ArkeCLR.Utilities.Extensions;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ArkeCLR.Runtime.Headers {
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

            this.Version = file.ReadString(Encoding.UTF8, this.VersionLength, 0);

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
