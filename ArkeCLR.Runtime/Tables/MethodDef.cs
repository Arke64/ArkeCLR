using ArkeCLR.Runtime.Flags;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct MethodDef : ICustomByteReader<TableStreamReader> {
        public uint RVA;
        public MethodImplAttributes ImplFlags;
        public MethodAttributes Flags;
        public HeapIndex Name;
        public HeapIndex Signature;
        public TableIndex ParamList;

        public void Read(TableStreamReader reader) {
            reader.Read(ref this.RVA);
            reader.ReadEnum(ref this.ImplFlags);
            reader.ReadEnum(ref this.Flags);
            reader.Read(ref this.Name, HeapType.String);
            reader.Read(ref this.Signature, HeapType.Blob);
            reader.Read(ref this.ParamList, TableType.Param);
        }
    }
}