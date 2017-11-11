using ArkeCLR.Runtime.Tables.Flags;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct ManifestResource : ICustomByteReader<TokenByteReader> {
        public uint Offset;
        public ManifestResourceAttributes Flags;
        public HeapToken Name;
        public TableToken Implementation;

        public void Read(TokenByteReader reader) {
            reader.Read(out this.Offset);
            reader.ReadEnum(out this.Flags);
            reader.Read(HeapType.String, out this.Name);
            reader.Read(CodedIndexType.Implementation, out this.Implementation);
        }
    }
}