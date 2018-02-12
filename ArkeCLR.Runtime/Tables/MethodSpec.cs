using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Tables {
    public struct MethodSpec : ICustomByteReader<TokenByteReader> {
        public TableToken Method;
        public HeapToken Instantiation;

        public void Read(TokenByteReader reader) {
            reader.Read(CodedIndexType.MethodDefOrRef, out this.Method);
            reader.Read(HeapType.Blob, out this.Instantiation);
        }
    }
}