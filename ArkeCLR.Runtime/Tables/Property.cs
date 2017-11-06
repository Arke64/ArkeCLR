using ArkeCLR.Runtime.Flags;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct Property : ICustomByteReader<IndexByteReader> {
        public PropertyAttributes Flags;
        public HeapIndex Name;
        public HeapIndex Type;

        public void Read(IndexByteReader reader) {
            reader.ReadEnum(out this.Flags);
            reader.Read(HeapType.String, out this.Name);
            reader.Read(HeapType.Blob, out this.Type);
        }
    }
}