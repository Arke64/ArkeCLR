using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct PropertyMap : ICustomByteReader<TokenByteReader> {
        public TableToken Parent;
        public TableToken PropertyList;

        public void Read(TokenByteReader reader) {
            reader.Read(TableType.TypeDef, out this.Parent);
            reader.Read(TableType.Property, out this.PropertyList);
        }
    }
}