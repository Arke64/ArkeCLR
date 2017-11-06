using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct MethodSpec : ICustomByteReader<IndexByteReader> {
        public TableIndex Method;
        public HeapIndex Instantiation;

        public void Read(IndexByteReader reader) {
            reader.Read(CodedIndexType.MethodDefOrRef, out this.Method);
            reader.Read(HeapType.Blob, out this.Instantiation);
        }
    }
}