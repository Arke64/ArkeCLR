using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Signatures.Types {
    public class Class : ICustomByteReader {
        public TypeDefOrRefOrSpecEncoded TypeDefOrRefOrSpecEncoded;

        public void Read(ByteReader reader) => reader.ReadCustom(out this.TypeDefOrRefOrSpecEncoded);
    }
}
