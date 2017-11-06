using ArkeCLR.Runtime.Signatures.Types;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Signatures {
    public class TypeSpec : ICustomByteReader {
        public ElementType ElementType;
        public Ptr Ptr;
        public FnPtr FnPtr;
        public Array Array;
        public SzArray SzArray;
        public GenericInst GenericInst;

        public void Read(ByteReader reader) {
            this.ElementType = reader.ReadEnum<ElementType>();

            switch (this.ElementType) {
                case ElementType.Ptr: reader.ReadCustom(out this.Ptr); break;
                case ElementType.FnPtr: reader.ReadCustom(out this.FnPtr); break;
                case ElementType.Array: reader.ReadCustom(out this.Array); break;
                case ElementType.SzArray: reader.ReadCustom(out this.SzArray); break;
                case ElementType.GenericInst: reader.ReadCustom(out this.GenericInst); break;
                default: throw new System.InvalidOperationException();
            }
        }
    }
}
