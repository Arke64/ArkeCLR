using ArkeCLR.Runtime.Flags;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct Param : ICustomByteReader<IndexByteReader> {
        public ParamAttributes Flags;
        public ushort Sequence;
        public HeapIndex Name;

        public void Read(IndexByteReader reader) {
            reader.ReadEnum(out this.Flags);
            reader.Read(out this.Sequence);
            reader.Read(HeapType.String, out this.Name);
        }
    }
}