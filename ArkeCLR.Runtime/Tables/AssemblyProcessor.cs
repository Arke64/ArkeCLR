using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct AssemblyProcessor : ICustomByteReader<TokenByteReader> {
        public uint Processor;

        public void Read(TokenByteReader reader) => reader.Read(out this.Processor);
    }
}