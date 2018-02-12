using ArkeCLR.Runtime.Streams;
using ArkeCLR.Runtime.Tables.Flags;
using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Tables {
    public struct Assembly : ICustomByteReader<TokenByteReader> {
        public AssemblyHashAlgorithm HashAlgId;
        public ushort MajorVersion;
        public ushort MinorVersion;
        public ushort BuildNumber;
        public ushort RevisionNumber;
        public AssemblyFlags Flags;
        public HeapToken PublicKey;
        public HeapToken Name;
        public HeapToken Culture;

        public void Read(TokenByteReader reader) {
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