using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Signatures {
    public struct TypeDefOrRefOrSpecEncoded {
        public TableType Table;
        public uint Row;

        public void Read(ByteReader reader) {
            var uncompressed = reader.ReadCompressedU4();
            var table = uncompressed & 0x03;

            this.Table = table == 0 ? TableType.TypeDef : (table == 1 ? TableType.TypeRef : TableType.TypeSpec);
            this.Row = (table & ~0x03U) >> 2;
        }

        public override string ToString() => $"{this.Table}->0x{this.Row:X8}";
    }
}
