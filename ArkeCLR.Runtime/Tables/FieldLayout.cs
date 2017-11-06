using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct FieldLayout : ICustomByteReader<IndexByteReader> {
        public uint Offset;
        public TableIndex Field;

        public void Read(IndexByteReader reader) {
            reader.Read(out this.Offset);
            reader.Read(TableType.Field, out this.Field);
        }
    }
}