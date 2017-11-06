using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct FieldMarshal : ICustomByteReader<IndexByteReader> {
        public TableIndex Parent;
        public HeapIndex NativeType;

        public void Read(IndexByteReader reader) {
            reader.Read(CodedIndexType.HasFieldMarshal, out this.Parent);
            reader.Read(HeapType.Blob, out this.NativeType);
        }
    }
}