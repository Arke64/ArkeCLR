using ArkeCLR.Runtime.Tables.Flags;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct Param : ICustomByteReader<TokenByteReader> {
        public ParamAttributes Flags;
        public ushort Sequence;
        public HeapToken Name;

        public void Read(TokenByteReader reader) {
            reader.ReadEnum(out this.Flags);
            reader.Read(out this.Sequence);
            reader.Read(HeapType.String, out this.Name);
        }
    }
}