using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct ModuleRef : ICustomByteReader<TokenByteReader> {
        public HeapToken Name;

        public void Read(TokenByteReader reader) => reader.Read(HeapType.String, out this.Name);
    }
}