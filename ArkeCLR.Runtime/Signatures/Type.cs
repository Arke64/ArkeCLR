using ArkeCLR.Runtime.Signatures.Types;
using ArkeCLR.Utilities;
using System;

namespace ArkeCLR.Runtime.Signatures {
    public struct Type {
        public ElementType ElementType;
        public SzArray SzArray;
        public Class Class;

        public Type(ElementType elementType) : this() => this.ElementType = elementType;

        public void Read(ElementType type, ByteReader reader) {
            this.ElementType = type;

            switch (this.ElementType) {
                case ElementType.Array: throw new NotImplementedException();
                case ElementType.Class: this.Class = new Class(reader); break;
                case ElementType.FnPtr: throw new NotImplementedException();
                case ElementType.GenericInst: throw new NotImplementedException();
                case ElementType.MVar: throw new NotImplementedException();
                case ElementType.Ptr: throw new NotImplementedException();
                case ElementType.SzArray: this.SzArray = new SzArray(reader); break;
                case ElementType.ValueType: throw new NotImplementedException();
                case ElementType.Var: throw new NotImplementedException();
            }
        }
    }
}
