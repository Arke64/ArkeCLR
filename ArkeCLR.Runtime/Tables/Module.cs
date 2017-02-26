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
            reader.Read(ref this.Generation);
            reader.Read(ref this.Name, HeapType.String);
            reader.Read(ref this.Mvid, HeapType.Guid);
            reader.Read(ref this.EncId, HeapType.Guid);
            reader.Read(ref this.EncBaseId, HeapType.Guid);
        }
    }
}
