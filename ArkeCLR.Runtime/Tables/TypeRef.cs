using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct TypeRef : ICustomByteReader<TableStreamReader> {
        public TableIndex ResolutionScope;
        public HeapIndex TypeName;
        public HeapIndex TypeNamespace;

        public void Read(TableStreamReader reader) {
            reader.Read(ref this.ResolutionScope, CodedIndexType.ResolutionScope);
            reader.Read(ref this.TypeName, HeapType.String);
            reader.Read(ref this.TypeNamespace, HeapType.String);
        }
    }
}
