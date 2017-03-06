using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct MemberRef : ICustomByteReader<TableStreamReader> {
        public TableIndex Class;
        public HeapIndex Name;
        public HeapIndex Signature;

        public void Read(TableStreamReader reader) {
            reader.Read(out this.Class, CodedIndexType.MemberRefParent);
            reader.Read(out this.Name, HeapType.String);
            reader.Read(out this.Signature, HeapType.Blob);
        }
    }
}