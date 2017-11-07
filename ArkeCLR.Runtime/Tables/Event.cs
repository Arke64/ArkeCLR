using ArkeCLR.Runtime.Tables.Flags;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct Event : ICustomByteReader<IndexByteReader> {
        public EventAttributes EventFlags;
        public HeapIndex Name;
        public TableIndex EventType;

        public void Read(IndexByteReader reader) {
            reader.ReadEnum(out this.EventFlags);
            reader.Read(HeapType.String, out this.Name);
            reader.Read(CodedIndexType.TypeDefOrRef, out this.EventType);
        }
    }
}