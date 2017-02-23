using ArkeCLR.Runtime.Streams;
using ArkeCLR.Runtime.TypeSystem;
using ArkeCLR.Utilities;
using System;

namespace ArkeCLR.Runtime.Tables {
    public struct TypeDef : ICustomByteReader<Assembly> {
        private Assembly parent;

        //TODO Need to add the actual enum
        public uint Flags;
        public uint TypeNameIdx;
        public uint TypeNamespaceIdx;
        public TableIndex ExtendsIdx;
        public TableIndex FieldListIdx;
        public TableIndex MethodListIdx;

        public string TypeName => this.parent.StringsStream.GetAt(this.TypeNameIdx);
        public string TypeNamespace => this.parent.StringsStream.GetAt(this.TypeNamespaceIdx);
        public object[] Fields => throw new NotImplementedException();
        public object[] Methods => throw new NotImplementedException();

        public override string ToString() => $"{this.TypeNamespace}.{this.TypeName}";

        public void Read(ByteReader reader, Assembly context) {
            this.parent = context;

            this.Flags = reader.ReadU4();
            this.TypeNameIdx = context.TablesStream.ReadHeapIndex(HeapType.String);
            this.TypeNamespaceIdx = context.TablesStream.ReadHeapIndex(HeapType.String);
            this.ExtendsIdx = context.TablesStream.ReadCodexIndex(CodedIndexType.TypeDefOrRef);
            this.FieldListIdx = context.TablesStream.ReadIndex(TableType.Field);
            this.MethodListIdx = context.TablesStream.ReadIndex(TableType.MethodDef);
        }
    }
}
