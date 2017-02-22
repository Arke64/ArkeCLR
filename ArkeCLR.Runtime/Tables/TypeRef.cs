using ArkeCLR.Runtime.FileFormats;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct TypeRef : ICustomByteReader<CliFile> {
        private CliFile parent;

        public TableIndex ResolutionScopeIdx;
        public uint TypeNameIdx;
        public uint TypeNamespaceIdx;

        public string TypeName => this.parent.StringsStream.GetAt(this.TypeNameIdx);
        public string TypeNamespace => this.parent.StringsStream.GetAt(this.TypeNamespaceIdx);

        public override string ToString() => $"{this.TypeNamespace}.{this.TypeName}";

        public void Read(ByteReader reader, CliFile context) {
            this.parent = context;

            this.ResolutionScopeIdx = context.TablesStream.ReadCodexIndex(CodedIndexType.ResolutionScope);
            this.TypeNameIdx = context.TablesStream.ReadHeapIndex(HeapType.String);
            this.TypeNamespaceIdx = context.TablesStream.ReadHeapIndex(HeapType.String);
        }
    }
}
