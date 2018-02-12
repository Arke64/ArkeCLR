using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Tables {
    public struct AssemblyOS : ICustomByteReader<TokenByteReader> {
        public uint OSPlatformID;
        public uint OSMajorVersion;
        public uint OSMinorVersion;

        public void Read(TokenByteReader reader) {
            reader.Read(out this.OSPlatformID);
            reader.Read(out this.OSMajorVersion);
            reader.Read(out this.OSMinorVersion);
        }
    }
}