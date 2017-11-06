using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct EventMap : ICustomByteReader<IndexByteReader> {
        public TableIndex Parent;
        public TableIndex EventList;

        public void Read(IndexByteReader reader) {
            reader.Read(TableType.TypeDef, out this.Parent);
            reader.Read(TableType.Event, out this.EventList);
        }
    }
}