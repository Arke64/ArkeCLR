using ArkeCLR.Runtime.Tables.Flags;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct File : ICustomByteReader<TokenByteReader> {
        public FileAttributes Flags;
        public HeapToken Name;
        public HeapToken HashValue;

        public void Read(TokenByteReader reader) {
            reader.ReadEnum(out this.Flags);
            reader.Read(HeapType.String, out this.Name);
            reader.Read(HeapType.Blob, out this.HashValue);
        }
    }
}