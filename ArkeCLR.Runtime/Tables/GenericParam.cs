using ArkeCLR.Runtime.Tables.Flags;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct GenericParam : ICustomByteReader<IndexByteReader> {
        public ushort Number;
        public GenericParamAttributes Flags;
        public TableIndex Owner;
        public HeapIndex Name;

        public void Read(IndexByteReader reader) {
            reader.Read(out this.Number);
            reader.ReadEnum(out this.Flags);
            reader.Read(CodedIndexType.TypeOfMethodDef, out this.Owner);
            reader.Read(HeapType.String, out this.Name);
        }
    }
}