using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct ModuleRef : ICustomByteReader<IndexByteReader> {
        public HeapIndex Name;

        public void Read(IndexByteReader reader) => reader.Read(HeapType.String, out this.Name);
    }
}