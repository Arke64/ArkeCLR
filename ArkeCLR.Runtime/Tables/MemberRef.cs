using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct MemberRef : ICustomByteReader<TableStreamReader> {
        public TableIndex Class;
        public uint Name;
        public uint Signature;

        public void Read(TableStreamReader reader) {
            reader.Read(ref this.Class, CodedIndexType.MemberRefParent);
            reader.Read(ref this.Name, HeapType.String);
            reader.Read(ref this.Signature, HeapType.Blob);
        }
    }
}