using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct AssemblyRefProcessor : ICustomByteReader<IndexByteReader> {
        public uint Processor;
        public TableIndex AssemblyRef;

        public void Read(IndexByteReader reader) {
            reader.Read(out this.Processor);
            reader.Read(TableType.AssemblyRef, out this.AssemblyRef);
        }
    }
}