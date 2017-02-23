using ArkeCLR.Runtime.Streams;
using ArkeCLR.Runtime.TypeSystem;
using ArkeCLR.Utilities;
using System;

namespace ArkeCLR.Runtime.Tables {
    public struct Module : ICustomByteReader<Assembly> {
        private Assembly parent;

        public ushort Generation;
        public uint NameIdx;
        public uint MvidIdx;
        public uint EncIdIdx;
        public uint EncBaseIdIdx;

        public string Name => this.parent.StringsStream.GetAt(this.NameIdx);
        public Guid? Mvid => this.parent.GuidStream.GetAt(this.MvidIdx);
        public Guid? EncId => this.parent.GuidStream.GetAt(this.EncIdIdx);
        public Guid? EncBaseId => this.parent.GuidStream.GetAt(this.EncBaseIdIdx);

        public override string ToString() => this.Name;

        public void Read(ByteReader reader, Assembly context) {
            this.parent = context;

            this.Generation = reader.ReadU2();

            this.NameIdx = context.TablesStream.ReadHeapIndex(HeapType.String);
            this.MvidIdx = context.TablesStream.ReadHeapIndex(HeapType.Guid);
            this.EncIdIdx = context.TablesStream.ReadHeapIndex(HeapType.Guid);
            this.EncBaseIdIdx = context.TablesStream.ReadHeapIndex(HeapType.Guid);
        }
    }
}
