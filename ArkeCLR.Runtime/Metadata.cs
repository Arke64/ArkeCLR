using ArkeCLR.Utilities;
using System.Runtime.InteropServices;
using System.Text;

namespace ArkeCLR.Runtime.Metadata {
    public struct RootHeader : ICustomByteReader {
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

    public struct StreamHeader : ICustomByteReader {
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
}
