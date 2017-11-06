using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct FieldRVA : ICustomByteReader<IndexByteReader> {
        public uint RVA;
        public TableIndex Field;

        public void Read(IndexByteReader reader) {
            reader.Read(out this.RVA);
            reader.Read(TableType.Field, out this.Field);
        }
    }
}