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
            reader.Read(ref this.Flags);
            reader.Read(ref this.TypeName, HeapType.String);
            reader.Read(ref this.TypeNamespace, HeapType.String);
            reader.Read(ref this.Extends, CodedIndexType.TypeDefOrRef);
            reader.Read(ref this.FieldList, TableType.Field);
            reader.Read(ref this.MethodList, TableType.MethodDef);
        }
    }
}
