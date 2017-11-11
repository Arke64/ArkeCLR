using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct Module : ICustomByteReader<TokenByteReader> {
        public ushort Generation;
        public HeapToken Name;
        public HeapToken Mvid;
        public HeapToken EncId;
        public HeapToken EncBaseId;

        public void Read(TokenByteReader reader) {
            reader.Read(out this.Generation);
            reader.Read(HeapType.String, out this.Name);
            reader.Read(HeapType.Guid, out this.Mvid);
            reader.Read(HeapType.Guid, out this.EncId);
            reader.Read(HeapType.Guid, out this.EncBaseId);
        }
    }
}
