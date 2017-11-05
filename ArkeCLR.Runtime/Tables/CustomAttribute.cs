using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct CustomAttribute : ICustomByteReader<IndexByteReader> {
        public TableIndex Parent;
        public TableIndex Type;
        public HeapIndex Value;

        public void Read(IndexByteReader reader) {
            reader.Read(CodedIndexType.HasCustomAttribute, out this.Parent);
            reader.Read(CodedIndexType.CustomAttributeType, out this.Type);
            reader.Read(HeapType.Blob, out this.Value);
        }
    }
}