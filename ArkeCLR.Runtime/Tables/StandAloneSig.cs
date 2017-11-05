using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct StandAloneSig : ICustomByteReader<IndexByteReader> {
        public HeapIndex Signature;

        public void Read(IndexByteReader reader) => reader.Read(HeapType.Blob, out this.Signature);
    }
}