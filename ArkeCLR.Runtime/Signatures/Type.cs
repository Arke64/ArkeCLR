using ArkeCLR.Runtime.Signatures.Types;
using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Signatures {
    public class Type : ICustomByteReader {
        public ElementType ElementType;
        public Array Array;
        public Class Class;
        public FnPtr FnPtr;
        public GenericInst GenericInst;
        public MVar MVar;
        public Ptr Ptr;
        public SzArray SzArray;
        public ValueType ValueType;
        public Var Var;

        public void Read(ByteReader reader) {
            this.ElementType = reader.ReadEnum<ElementType>();

            switch (this.ElementType) {
                case ElementType.Array: reader.ReadCustom(out this.Array); break;
                case ElementType.Class: reader.ReadCustom(out this.Class); break;
                case ElementType.FnPtr: reader.ReadCustom(out this.FnPtr); break;
                case ElementType.GenericInst: reader.ReadCustom(out this.GenericInst); break;
                case ElementType.MVar: reader.ReadCustom(out this.MVar); break;
                case ElementType.Ptr: reader.ReadCustom(out this.Ptr); break;
                case ElementType.SzArray: reader.ReadCustom(out this.SzArray); break;
                case ElementType.ValueType: reader.ReadCustom(out this.ValueType); break;
                case ElementType.Var: reader.ReadCustom(out this.Var); break;
            }
        }
    }
}
