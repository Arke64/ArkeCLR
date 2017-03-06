using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct Module : ICustomByteReader<TableStreamReader> {
        public ushort Generation;
        public HeapIndex Name;
        public HeapIndex Mvid;
        public HeapIndex EncId;
        public HeapIndex EncBaseId;

        public void Read(TableStreamReader reader) {
            reader.Read(out this.Generation);
            reader.Read(out this.Name, HeapType.String);
            reader.Read(out this.Mvid, HeapType.Guid);
            reader.Read(out this.EncId, HeapType.Guid);
            reader.Read(out this.EncBaseId, HeapType.Guid);
        }
    }
}
