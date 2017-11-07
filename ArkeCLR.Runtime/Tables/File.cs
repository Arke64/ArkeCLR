using ArkeCLR.Runtime.Tables.Flags;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct File : ICustomByteReader<IndexByteReader> {
        public FileAttributes Flags;
        public HeapIndex Name;
        public HeapIndex HashValue;

        public void Read(IndexByteReader reader) {
            reader.ReadEnum(out this.Flags);
            reader.Read(HeapType.String, out this.Name);
            reader.Read(HeapType.Blob, out this.HashValue);
        }
    }
}