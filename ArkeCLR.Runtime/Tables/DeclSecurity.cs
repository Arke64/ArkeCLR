using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct DeclSecurity : ICustomByteReader<IndexByteReader> {
        public ushort Action;
        public TableIndex Parent;
        public HeapIndex PermissionSet;

        public void Read(IndexByteReader reader) {
            reader.Read(out this.Action);
            reader.Read(CodedIndexType.HasDeclSecurity, out this.Parent);
            reader.Read(HeapType.Blob, out this.PermissionSet);
        }
    }
}