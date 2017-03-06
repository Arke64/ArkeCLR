using ArkeCLR.Runtime.Flags;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct Param : ICustomByteReader<TableStreamReader> {
        public ParamAttributes Flags;
        public ushort Sequence;
        public HeapIndex Name;

        public void Read(TableStreamReader reader) {
            reader.ReadEnum(out this.Flags);
            reader.Read(out this.Sequence);
            reader.Read(out this.Name, HeapType.String);
        }
    }
}