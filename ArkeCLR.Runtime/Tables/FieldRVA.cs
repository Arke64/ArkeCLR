using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct FieldRVA : ICustomByteReader<TokenByteReader> {
        public uint RVA;
        public TableToken Field;

        public void Read(TokenByteReader reader) {
            reader.Read(out this.RVA);
            reader.Read(TableType.Field, out this.Field);
        }
    }
}