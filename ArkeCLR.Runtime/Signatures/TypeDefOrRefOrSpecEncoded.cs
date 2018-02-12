using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Signatures {
    public class TypeDefOrRefOrSpecEncoded : ICustomByteReader {
        public TableToken Token;

        public void Read(ByteReader reader) {
            var uncompressed = reader.ReadCompressedU4();

            this.Token = new TableToken { Row = uncompressed >> 2 };

            uncompressed &= 0x03;

            this.Token.Table = uncompressed == 0 ? TableType.TypeDef : (uncompressed == 1 ? TableType.TypeRef : TableType.TypeSpec);
        }
    }
}
