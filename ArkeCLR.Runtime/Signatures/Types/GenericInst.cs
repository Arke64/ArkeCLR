using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Signatures.Types {
    public class GenericInst : ICustomByteReader {
        public ElementType ElementType;
        public TypeDefOrRefOrSpecEncoded TypeDefOrRefOrSpecEncoded;
        public uint GenArgCount;
        public Type[] Types;

        public void Read(ByteReader reader) {
            reader.ReadEnum(out this.ElementType);
            reader.ReadCustom(out this.TypeDefOrRefOrSpecEncoded);

            this.GenArgCount = reader.ReadCompressedU4();

            reader.ReadCustom(this.GenArgCount, out this.Types);
        }
    }
}
