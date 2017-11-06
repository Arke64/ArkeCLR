using ArkeCLR.Runtime.Signatures.Types;
using ArkeCLR.Utilities;
using System;

namespace ArkeCLR.Runtime.Signatures {
    public class Type : ICustomByteReader {
        public ElementType ElementType;
        public SzArray SzArray;
        public Class Class;

        public void Read(ByteReader reader) {
            this.ElementType = reader.ReadEnum<ElementType>();

            switch (this.ElementType) {
                case ElementType.Array: throw new NotImplementedException();
                case ElementType.Class: reader.ReadCustom(out this.Class); break;
                case ElementType.FnPtr: throw new NotImplementedException();
                case ElementType.GenericInst: throw new NotImplementedException();
                case ElementType.MVar: throw new NotImplementedException();
                case ElementType.Ptr: throw new NotImplementedException();
                case ElementType.SzArray: reader.ReadCustom(out this.SzArray); break;
                case ElementType.ValueType: throw new NotImplementedException();
                case ElementType.Var: throw new NotImplementedException();
            }
        }
    }
}
