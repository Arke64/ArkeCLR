using ArkeCLR.Runtime.Streams;
using ArkeCLR.Runtime.Tables.Flags;
using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Tables {
    public struct TypeDef : ICustomByteReader<TokenByteReader> {
        public TypeAttributes Flags;
        public HeapToken TypeName;
        public HeapToken TypeNamespace;
        public TableToken Extends;
        public TableToken FieldList;
        public TableToken MethodList;

        public void Read(TokenByteReader reader) {
            reader.ReadEnum(out this.Flags);
            reader.Read(HeapType.String, out this.TypeName);
            reader.Read(HeapType.String, out this.TypeNamespace);
            reader.Read(CodedIndexType.TypeDefOrRef, out this.Extends);
            reader.Read(TableType.Field, out this.FieldList);
            reader.Read(TableType.MethodDef, out this.MethodList);
        }
    }
}
