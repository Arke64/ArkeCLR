using ArkeCLR.Runtime.Signatures.Types;
using ArkeCLR.Utilities;
using System;

namespace ArkeCLR.Runtime.Signatures {
    public struct Type {
        public ElementType ElementType;
        public SzArrayType SzArray;

        public void Read(ElementType type, ByteReader reader) {
            this.ElementType = type;

            switch (this.ElementType) {
                case ElementType.Array: throw new NotImplementedException();
                case ElementType.Class: throw new NotImplementedException();
                case ElementType.FnPtr: throw new NotImplementedException();
                case ElementType.GenericInst: throw new NotImplementedException();
                case ElementType.MVar: throw new NotImplementedException();
                case ElementType.Ptr: throw new NotImplementedException();
                case ElementType.SzArray: this.SzArray = new SzArrayType(reader); break;
                case ElementType.ValueType: throw new NotImplementedException();
                case ElementType.Var: throw new NotImplementedException();
            }
        }
    }
}
