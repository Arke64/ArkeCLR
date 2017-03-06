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
            reader.Read(out this.RVA);
            reader.ReadEnum(out this.ImplFlags);
            reader.ReadEnum(out this.Flags);
            reader.Read(out this.Name, HeapType.String);
            reader.Read(out this.Signature, HeapType.Blob);
            reader.Read(out this.ParamList, TableType.Param);
        }
    }
}