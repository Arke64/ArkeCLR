using ArkeCLR.Runtime.Flags;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct ManifestResource : ICustomByteReader<IndexByteReader> {
        public uint Offset;
        public ManifestResourceAttributes Flags;
        public HeapIndex Name;
        public TableIndex Implementation;

        public void Read(IndexByteReader reader) {
            reader.Read(out this.Offset);
            reader.ReadEnum(out this.Flags);
            reader.Read(HeapType.String, out this.Name);
            reader.Read(CodedIndexType.Implementation, out this.Implementation);
        }
    }
}