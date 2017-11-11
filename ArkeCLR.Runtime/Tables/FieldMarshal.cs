using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct FieldMarshal : ICustomByteReader<TokenByteReader> {
        public TableToken Parent;
        public HeapToken NativeType;

        public void Read(TokenByteReader reader) {
            reader.Read(CodedIndexType.HasFieldMarshal, out this.Parent);
            reader.Read(HeapType.Blob, out this.NativeType);
        }
    }
}