using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Tables {
    public struct FieldLayout : ICustomByteReader<TokenByteReader> {
        public uint Offset;
        public TableToken Field;

        public void Read(TokenByteReader reader) {
            reader.Read(out this.Offset);
            reader.Read(TableType.Field, out this.Field);
        }
    }
}