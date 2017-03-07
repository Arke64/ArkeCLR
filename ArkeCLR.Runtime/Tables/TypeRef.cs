using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct TypeRef : ICustomByteReader<IndexByteReader> {
        public TableIndex ResolutionScope;
        public HeapIndex TypeName;
        public HeapIndex TypeNamespace;

        public void Read(IndexByteReader reader) {
            reader.Read(out this.ResolutionScope, CodedIndexType.ResolutionScope);
            reader.Read(out this.TypeName, HeapType.String);
            reader.Read(out this.TypeNamespace, HeapType.String);
        }
    }
}
