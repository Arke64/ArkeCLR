using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Tables {
    public struct ModuleRef : ICustomByteReader<TokenByteReader> {
        public HeapToken Name;

        public void Read(TokenByteReader reader) => reader.Read(HeapType.String, out this.Name);
    }
}