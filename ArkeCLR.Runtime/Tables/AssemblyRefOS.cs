using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct AssemblyRefOS : ICustomByteReader<TokenByteReader> {
        public uint OSPlatformID;
        public uint OSMajorVersion;
        public uint OSMinorVersion;
        public TableToken AssemblyRef;

        public void Read(TokenByteReader reader) {
            reader.Read(out this.OSPlatformID);
            reader.Read(out this.OSMajorVersion);
            reader.Read(out this.OSMinorVersion);
            reader.Read(TableType.AssemblyRef, out this.AssemblyRef);
        }
    }
}