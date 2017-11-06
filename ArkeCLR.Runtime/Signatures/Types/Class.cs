using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Signatures.Types {
    public class Class {
        public TypeDefOrRefOrSpecEncoded TypeDefOrRefOrSpecEncoded;

        public Class(ByteReader reader) => this.TypeDefOrRefOrSpecEncoded.Read(reader);
    }
}
