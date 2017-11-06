using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Signatures {
    public class TypeDefOrRefOrSpecEncoded : ICustomByteReader {
        public TableIndex Index;

        public void Read(ByteReader reader) {
            var uncompressed = reader.ReadCompressedU4();

            this.Index = new TableIndex { Row = uncompressed >> 2 };

            uncompressed &= 0x03;

            this.Index.Table = uncompressed == 0 ? TableType.TypeDef : (uncompressed == 1 ? TableType.TypeRef : TableType.TypeSpec);
        }
    }
}
