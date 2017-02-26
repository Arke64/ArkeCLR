using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct CustomAttribute : ICustomByteReader<TableStreamReader> {
        public TableIndex Parent;
        public TableIndex Type;
        public HeapIndex Value;

        public void Read(TableStreamReader reader) {
            reader.Read(ref this.Parent, CodedIndexType.HasCustomAttribute);
            reader.Read(ref this.Type, CodedIndexType.CustomAttributeType);
            reader.Read(ref this.Value, HeapType.Blob);
        }
    }
}