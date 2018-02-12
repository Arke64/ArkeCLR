using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Tables {
    public struct CustomAttribute : ICustomByteReader<TokenByteReader> {
        public TableToken Parent;
        public TableToken Type;
        public HeapToken Value;

        public void Read(TokenByteReader reader) {
            reader.Read(CodedIndexType.HasCustomAttribute, out this.Parent);
            reader.Read(CodedIndexType.CustomAttributeType, out this.Type);
            reader.Read(HeapType.Blob, out this.Value);
        }
    }
}