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
            reader.Read(out this.TypeName, HeapType.String);
            reader.Read(out this.TypeNamespace, HeapType.String);
            reader.Read(out this.Extends, CodedIndexType.TypeDefOrRef);
            reader.Read(out this.FieldList, TableType.Field);
            reader.Read(out this.MethodList, TableType.MethodDef);
        }
    }
}
