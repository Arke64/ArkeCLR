using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct TypeRef : ICustomByteReader<IndexByteReader> {
        public TableIndex ResolutionScope;
        public HeapIndex TypeName;
        public HeapIndex TypeNamespace;

        public void Read(IndexByteReader reader) {
            reader.Read(CodedIndexType.ResolutionScope, out this.ResolutionScope);
            reader.Read(HeapType.String, out this.TypeName);
            reader.Read(HeapType.String, out this.TypeNamespace);
        }
    }
}
