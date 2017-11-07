using ArkeCLR.Runtime.Tables.Flags;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct Assembly : ICustomByteReader<IndexByteReader> {
        public AssemblyHashAlgorithm HashAlgId;
        public ushort MajorVersion;
        public ushort MinorVersion;
        public ushort BuildNumber;
        public ushort RevisionNumber;
        public AssemblyFlags Flags;
        public HeapIndex PublicKey;
        public HeapIndex Name;
        public HeapIndex Culture;

        public void Read(IndexByteReader reader) {
            reader.ReadEnum(out this.HashAlgId);
            reader.Read(out this.MajorVersion);
            reader.Read(out this.MinorVersion);
            reader.Read(out this.BuildNumber);
            reader.Read(out this.RevisionNumber);
            reader.ReadEnum(out this.Flags);
            reader.Read(HeapType.Blob, out this.PublicKey);
            reader.Read(HeapType.String, out this.Name);
            reader.Read(HeapType.String, out this.Culture);
        }
    }
}