using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct MemberRef : ICustomByteReader<IndexByteReader> {
        public TableIndex Class;
        public HeapIndex Name;
        public HeapIndex Signature;

        public void Read(IndexByteReader reader) {
            reader.Read(CodedIndexType.MemberRefParent, out this.Class);
            reader.Read(HeapType.String, out this.Name);
            reader.Read(HeapType.Blob, out this.Signature);
        }
    }
}