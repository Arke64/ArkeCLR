using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct AssemblyRefProcessor : ICustomByteReader<TokenByteReader> {
        public uint Processor;
        public TableToken AssemblyRef;

        public void Read(TokenByteReader reader) {
            reader.Read(out this.Processor);
            reader.Read(TableType.AssemblyRef, out this.AssemblyRef);
        }
    }
}