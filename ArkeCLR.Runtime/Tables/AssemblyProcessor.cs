using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Tables {
    public struct AssemblyProcessor : ICustomByteReader<TokenByteReader> {
        public uint Processor;

        public void Read(TokenByteReader reader) => reader.Read(out this.Processor);
    }
}