using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct TypeRef : ICustomByteReader<TableStreamReader> {
        public TableIndex ResolutionScope;
        public uint TypeName;
        public uint TypeNamespace;

        public void Read(TableStreamReader reader) {
            this.ResolutionScope = reader.ReadCodedIndex(CodedIndexType.ResolutionScope);
            this.TypeName = reader.ReadHeapIndex(HeapType.String);
            this.TypeNamespace = reader.ReadHeapIndex(HeapType.String);
        }
    }
}
