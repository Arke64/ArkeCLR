using ArkeCLR.Runtime.Tables.Flags;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct ImplMap : ICustomByteReader<IndexByteReader> {
        public PInvokeAttributes MappingFlags;
        public TableIndex MemberForwarded;
        public HeapIndex ImportName;
        public TableIndex ImportScope;

        public void Read(IndexByteReader reader) {
            reader.ReadEnum(out this.MappingFlags);
            reader.Read(CodedIndexType.MemberForwarded, out this.MemberForwarded);
            reader.Read(HeapType.String, out this.ImportName);
            reader.Read(TableType.ModuleRef, out this.ImportScope);
        }
    }
}