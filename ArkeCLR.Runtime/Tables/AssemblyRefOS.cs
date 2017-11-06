using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct AssemblyRefOS : ICustomByteReader<IndexByteReader> {
        public uint OSPlatformID;
        public uint OSMajorVersion;
        public uint OSMinorVersion;
        public TableIndex AssemblyRef;

        public void Read(IndexByteReader reader) {
            reader.Read(out this.OSPlatformID);
            reader.Read(out this.OSMajorVersion);
            reader.Read(out this.OSMinorVersion);
            reader.Read(TableType.AssemblyRef, out this.AssemblyRef);
        }
    }
}