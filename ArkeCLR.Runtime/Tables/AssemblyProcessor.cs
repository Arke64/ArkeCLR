using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct AssemblyProcessor : ICustomByteReader<IndexByteReader> {
        public uint Processor;

        public void Read(IndexByteReader reader) => reader.Read(out this.Processor);
    }
}