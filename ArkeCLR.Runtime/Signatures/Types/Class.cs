using ArkeCLR.Utilities;
using ArkeCLR.Utilities.Extensions;

namespace ArkeCLR.Runtime.Signatures.Types {
    public class Class {
        public TypeDefOrRefOrSpecEncoded TypeDefOrRefOrSpecEncoded;

        public Class(ByteReader reader) => this.TypeDefOrRefOrSpecEncoded.Read(reader);

        public override string ToString() => this.TypeDefOrRefOrSpecEncoded.ToString();
    }
}
