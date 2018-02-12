using ArkeCLR.Runtime.Streams;
using ArkeCLR.Runtime.Tables.Flags;
using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Tables {
    public struct ImplMap : ICustomByteReader<TokenByteReader> {
        public PInvokeAttributes MappingFlags;
        public TableToken MemberForwarded;
        public HeapToken ImportName;
        public TableToken ImportScope;

        public void Read(TokenByteReader reader) {
            reader.ReadEnum(out this.MappingFlags);
            reader.Read(CodedIndexType.MemberForwarded, out this.MemberForwarded);
            reader.Read(HeapType.String, out this.ImportName);
            reader.Read(TableType.ModuleRef, out this.ImportScope);
        }
    }
}