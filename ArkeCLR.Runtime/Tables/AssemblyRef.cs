using ArkeCLR.Runtime.Streams;
using ArkeCLR.Runtime.Tables.Flags;
using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Tables {
    public struct AssemblyRef : ICustomByteReader<TokenByteReader> {
        public ushort MajorVersion;
        public ushort MinorVersion;
        public ushort BuildNumber;
        public ushort RevisionNumber;
        public AssemblyFlags Flags;
        public HeapToken PublicKeyOrToken;
        public HeapToken Name;
        public HeapToken Culture;
        public HeapToken HashValue;

        public void Read(TokenByteReader reader) {
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
