using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Signatures {
    public class Constraint : ICustomByteReader {
        public ElementType Type;

        public void Read(ByteReader reader) => this.Type = reader.ReadEnum<ElementType>();
    }
}
