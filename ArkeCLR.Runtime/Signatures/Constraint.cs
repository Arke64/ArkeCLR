using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Signatures {
    public class Constraint : ICustomByteReader {
        public ElementType ElementType;

        public void Read(ByteReader reader) => this.ElementType = reader.ReadEnum<ElementType>();
    }
}
