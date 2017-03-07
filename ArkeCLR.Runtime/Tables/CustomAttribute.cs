using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct CustomAttribute : ICustomByteReader<IndexByteReader> {
        public TableIndex Parent;
        public TableIndex Type;
        public HeapIndex Value;

        public void Read(IndexByteReader reader) {
            reader.Read(out this.Parent, CodedIndexType.HasCustomAttribute);
            reader.Read(out this.Type, CodedIndexType.CustomAttributeType);
            reader.Read(out this.Value, HeapType.Blob);
        }
    }
}