using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Tables {
    public struct EventMap : ICustomByteReader<TokenByteReader> {
        public TableToken Parent;
        public TableToken EventList;

        public void Read(TokenByteReader reader) {
            reader.Read(TableType.TypeDef, out this.Parent);
            reader.Read(TableType.Event, out this.EventList);
        }
    }
}