using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct Module : ICustomByteReader<TableStreamReader> {
        public ushort Generation;
        public uint Name;
        public uint Mvid;
        public uint EncId;
        public uint EncBaseId;

        public void Read(TableStreamReader reader) {
            this.Generation = reader.ReadU2();

            this.Name = reader.ReadHeapIndex(HeapType.String);
            this.Mvid = reader.ReadHeapIndex(HeapType.Guid);
            this.EncId = reader.ReadHeapIndex(HeapType.Guid);
            this.EncBaseId = reader.ReadHeapIndex(HeapType.Guid);
        }
    }
}
