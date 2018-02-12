using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Tables {
    public struct MemberRef : ICustomByteReader<TokenByteReader> {
        public TableToken Class;
        public HeapToken Name;
        public HeapToken Signature;

        public void Read(TokenByteReader reader) {
            reader.Read(CodedIndexType.MemberRefParent, out this.Class);
            reader.Read(HeapType.String, out this.Name);
            reader.Read(HeapType.Blob, out this.Signature);
        }
    }
}