using ArkeCLR.Runtime.Signatures.Types;
using ArkeCLR.Utilities;
using System;

namespace ArkeCLR.Runtime.Signatures {
    public struct Type {
        public ElementType ElementType;
        public SzArrayType SzArray;

        public Type(ElementType elementType) : this() => this.ElementType = elementType;

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

        public override string ToString() {
            switch (this.ElementType) {
                case ElementType.Array: throw new NotImplementedException();
                case ElementType.Class: throw new NotImplementedException();
                case ElementType.FnPtr: throw new NotImplementedException();
                case ElementType.GenericInst: throw new NotImplementedException();
                case ElementType.MVar: throw new NotImplementedException();
                case ElementType.Ptr: throw new NotImplementedException();
                case ElementType.SzArray: return this.SzArray.ToString();
                case ElementType.ValueType: throw new NotImplementedException();
                case ElementType.Var: throw new NotImplementedException();
                default: return this.ElementType.ToString();
            }
        }

        public bool Equals(Type obj) => obj == this;
        public override bool Equals(object obj) => obj != null && this.Equals((Type)obj);
        public override int GetHashCode() => this.ElementType.GetHashCode();

        public static bool operator !=(Type self, Type other) => !(self.ElementType == other.ElementType);
        public static bool operator ==(Type self, Type other) => self.ElementType == other.ElementType;
    }
}
