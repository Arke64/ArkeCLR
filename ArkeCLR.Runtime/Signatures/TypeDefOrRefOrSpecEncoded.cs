using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Signatures {
    public struct TypeDefOrRefOrSpecEncoded {
        public TableType Table;
        public uint Row;

        public void Read(ByteReader reader) {
            var uncompressed = reader.ReadCompressedU4();

            this.Row = uncompressed >> 2;

            uncompressed &= 0x03;

            this.Table = uncompressed == 0 ? TableType.TypeDef : (uncompressed == 1 ? TableType.TypeRef : TableType.TypeSpec);
        }
    }
}
