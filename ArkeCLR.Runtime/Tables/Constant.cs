using ArkeCLR.Runtime.Signatures;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Tables {
    public struct Constant : ICustomByteReader<TokenByteReader> {
        public ElementType Type;
        public byte Padding;
        public TableToken Parent;
        public HeapToken Value;

        public void Read(TokenByteReader reader) {
            reader.ReadEnum(out this.Type);
            reader.Read(out this.Padding);
            reader.Read(CodedIndexType.HasConstant, out this.Parent);
            reader.Read(HeapType.Blob, out this.Value);
        }
    }
}