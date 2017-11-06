using ArkeCLR.Runtime.Flags;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct ExportedType : ICustomByteReader<IndexByteReader> {
        public TypeAttributes Flags;
        public TableIndex TypeDefId;
        public HeapIndex TypeName;
        public HeapIndex TypeNamespace;
        public TableIndex Implementation;

        public void Read(IndexByteReader reader) {
            reader.ReadEnum(out this.Flags);
            reader.Read(TableType.TypeDef, out this.TypeDefId);
            reader.Read(HeapType.String, out this.TypeName);
            reader.Read(HeapType.String, out this.TypeNamespace);
            reader.Read(CodedIndexType.Implementation, out this.Implementation);
        }
    }
}