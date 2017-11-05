using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct Module : ICustomByteReader<IndexByteReader> {
        public ushort Generation;
        public HeapIndex Name;
        public HeapIndex Mvid;
        public HeapIndex EncId;
        public HeapIndex EncBaseId;

        public void Read(IndexByteReader reader) {
            reader.Read(out this.Generation);
            reader.Read(HeapType.String, out this.Name);
            reader.Read(HeapType.Guid, out this.Mvid);
            reader.Read(HeapType.Guid, out this.EncId);
            reader.Read(HeapType.Guid, out this.EncBaseId);
        }
    }
}
