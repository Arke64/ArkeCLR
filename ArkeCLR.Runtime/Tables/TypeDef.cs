using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct TypeDef : ICustomByteReader<TableStreamReader> {
        //TODO Need to add the actual enum
        public uint Flags;
        public uint TypeName;
        public uint TypeNamespace;
        public TableIndex Extends;
        public TableIndex FieldList;
        public TableIndex MethodList;

        public void Read(TableStreamReader reader) {
            this.Flags = reader.ReadU4();
            this.TypeName = reader.ReadIndex(HeapType.String);
            this.TypeNamespace = reader.ReadIndex(HeapType.String);
            this.Extends = reader.ReadIndex(CodedIndexType.TypeDefOrRef);
            this.FieldList = reader.ReadIndex(TableType.Field);
            this.MethodList = reader.ReadIndex(TableType.MethodDef);
        }
    }
}
