using ArkeCLR.Runtime.Flags;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct AssemblyRef : ICustomByteReader<TableStreamReader> {
        public ushort MajorVersion;
        public ushort MinorVersion;
        public ushort BuildNumber;
        public ushort RevisionNumber;
        public AssemblyFlags Flags;
        public HeapIndex PublicKeyOrToken;
        public HeapIndex Name;
        public HeapIndex Culture;
        public HeapIndex HashValue;

        public void Read(TableStreamReader reader) {
            reader.Read(out this.MajorVersion);
            reader.Read(out this.MinorVersion);
            reader.Read(out this.BuildNumber);
            reader.Read(out this.RevisionNumber);
            reader.ReadEnum(out this.Flags);
            reader.Read(out this.PublicKeyOrToken, HeapType.Blob);
            reader.Read(out this.Name, HeapType.String);
            reader.Read(out this.Culture, HeapType.String);
            reader.Read(out this.HashValue, HeapType.Blob);
        }
    }
}
