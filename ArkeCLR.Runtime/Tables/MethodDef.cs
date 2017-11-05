using ArkeCLR.Runtime.Flags;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct MethodDef : ICustomByteReader<IndexByteReader> {
        public uint RVA;
        public MethodImplAttributes ImplFlags;
        public MethodAttributes Flags;
        public HeapIndex Name;
        public HeapIndex Signature;
        public TableIndex ParamList;

        public void Read(IndexByteReader reader) {
            reader.Read(out this.RVA);
            reader.ReadEnum(out this.ImplFlags);
            reader.ReadEnum(out this.Flags);
            reader.Read(HeapType.String, out this.Name);
            reader.Read(HeapType.Blob, out this.Signature);
            reader.Read(TableType.Param, out this.ParamList);
        }
    }
}