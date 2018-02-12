using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Signatures.Types {
    public class Array : ICustomByteReader {
        public Type Type;
        public ArrayShape ArrayShape;

        public void Read(ByteReader reader) {
            reader.ReadCustom(out this.Type);
            reader.ReadCustom(out this.ArrayShape);
        }
    }
}
