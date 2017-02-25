using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct TypeRef : ICustomByteReader<TableStreamReader> {
        public TableIndex ResolutionScope;
        public uint TypeName;
        public uint TypeNamespace;

        public void Read(TableStreamReader reader) {
            this.ResolutionScope = reader.ReadIndex(CodedIndexType.ResolutionScope);
            this.TypeName = reader.ReadIndex(HeapType.String);
            this.TypeNamespace = reader.ReadIndex(HeapType.String);
        }
    }
}
