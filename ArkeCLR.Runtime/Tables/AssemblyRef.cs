using ArkeCLR.Runtime.Tables.Flags;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct AssemblyRef : ICustomByteReader<IndexByteReader> {
        public ushort MajorVersion;
        public ushort MinorVersion;
        public ushort BuildNumber;
        public ushort RevisionNumber;
        public AssemblyFlags Flags;
        public HeapIndex PublicKeyOrToken;
        public HeapIndex Name;
        public HeapIndex Culture;
        public HeapIndex HashValue;

        public void Read(IndexByteReader reader) {
            reader.Read(out this.MajorVersion);
            reader.Read(out this.MinorVersion);
            reader.Read(out this.BuildNumber);
            reader.Read(out this.RevisionNumber);
            reader.ReadEnum(out this.Flags);
            reader.Read(HeapType.Blob, out this.PublicKeyOrToken);
            reader.Read(HeapType.String, out this.Name);
            reader.Read(HeapType.String, out this.Culture);
            reader.Read(HeapType.Blob, out this.HashValue);
        }
    }
}
