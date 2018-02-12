using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Signatures {
    public class Constraint : ICustomByteReader {
        public ElementType ElementType;

        public void Read(ByteReader reader) => this.ElementType = reader.ReadEnum<ElementType>();
    }
}
