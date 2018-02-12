using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Tables {
    public struct TypeRef : ICustomByteReader<TokenByteReader> {
        public TableToken ResolutionScope;
        public HeapToken TypeName;
        public HeapToken TypeNamespace;

        public void Read(TokenByteReader reader) {
            reader.Read(CodedIndexType.ResolutionScope, out this.ResolutionScope);
            reader.Read(HeapType.String, out this.TypeName);
            reader.Read(HeapType.String, out this.TypeNamespace);
        }
    }
}
