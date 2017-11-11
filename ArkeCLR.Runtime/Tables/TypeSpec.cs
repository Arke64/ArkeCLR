using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct TypeSpec : ICustomByteReader<TokenByteReader> {
        public HeapToken Signature;

        public void Read(TokenByteReader reader) => reader.Read(HeapType.Blob, out this.Signature);
    }
}