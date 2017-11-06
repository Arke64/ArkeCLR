using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct NestedClass : ICustomByteReader<IndexByteReader> {
        //Members cannot have the same name as their enclosing type.
        public TableIndex NestedCls;
        public TableIndex EnclosingCls;

        public void Read(IndexByteReader reader) {
            reader.Read(TableType.TypeDef, out this.NestedCls);
            reader.Read(TableType.TypeDef, out this.EnclosingCls);
        }
    }
}