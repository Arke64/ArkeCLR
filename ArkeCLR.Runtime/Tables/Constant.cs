using ArkeCLR.Runtime.Signatures;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct Constant : ICustomByteReader<IndexByteReader> {
        public ElementType Type;
        public byte Padding;
        public TableIndex Parent;
        public HeapIndex Value;

        public void Read(IndexByteReader reader) {
            reader.ReadEnum(out this.Type);
            reader.Read(out this.Padding);
            reader.Read(CodedIndexType.HasConstant, out this.Parent);
            reader.Read(HeapType.Blob, out this.Value);
        }
    }
}