using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Signatures.Types {
    public class ValueType : ICustomByteReader {
        public TypeDefOrRefOrSpecEncoded TypeDefOrRefOrSpecEncoded;

        public void Read(ByteReader reader) => reader.ReadCustom(out this.TypeDefOrRefOrSpecEncoded);
    }
}
