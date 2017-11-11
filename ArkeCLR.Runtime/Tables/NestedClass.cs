using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct NestedClass : ICustomByteReader<TokenByteReader> {
        //Members cannot have the same name as their enclosing type.
        public TableToken NestedCls;
        public TableToken EnclosingCls;

        public void Read(TokenByteReader reader) {
            reader.Read(TableType.TypeDef, out this.NestedCls);
            reader.Read(TableType.TypeDef, out this.EnclosingCls);
        }
    }
}