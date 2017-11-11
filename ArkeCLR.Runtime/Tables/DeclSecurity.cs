using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct DeclSecurity : ICustomByteReader<TokenByteReader> {
        public ushort Action;
        public TableToken Parent;
        public HeapToken PermissionSet;

        public void Read(TokenByteReader reader) {
            reader.Read(out this.Action);
            reader.Read(CodedIndexType.HasDeclSecurity, out this.Parent);
            reader.Read(HeapType.Blob, out this.PermissionSet);
        }
    }
}