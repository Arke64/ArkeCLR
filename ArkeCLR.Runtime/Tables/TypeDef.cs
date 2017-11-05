using ArkeCLR.Runtime.Flags;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct TypeDef : ICustomByteReader<IndexByteReader> {
        public TypeAttributes Flags;
        public HeapIndex TypeName;
        public HeapIndex TypeNamespace;
        public TableIndex Extends;
        public TableIndex FieldList;
        public TableIndex MethodList;

        public void Read(IndexByteReader reader) {
            reader.ReadEnum(out this.Flags);
            reader.Read(HeapType.String, out this.TypeName);
            reader.Read(HeapType.String, out this.TypeNamespace);
            reader.Read(CodedIndexType.TypeDefOrRef, out this.Extends);
            reader.Read(TableType.Field, out this.FieldList);
            reader.Read(TableType.MethodDef, out this.MethodList);
        }
    }
}
