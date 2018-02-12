using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

#pragma warning disable S1104

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