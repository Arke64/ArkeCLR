using ArkeCLR.Runtime.Tables.Flags;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct ExportedType : ICustomByteReader<TokenByteReader> {
        public TypeAttributes Flags;
        public TableToken TypeDefId;
        public HeapToken TypeName;
        public HeapToken TypeNamespace;
        public TableToken Implementation;

        public void Read(TokenByteReader reader) {
            reader.ReadEnum(out this.Flags);
            reader.Read(TableType.TypeDef, out this.TypeDefId);
            reader.Read(HeapType.String, out this.TypeName);
            reader.Read(HeapType.String, out this.TypeNamespace);
            reader.Read(CodedIndexType.Implementation, out this.Implementation);
        }
    }
}