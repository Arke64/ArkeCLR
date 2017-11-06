using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct PropertyMap : ICustomByteReader<IndexByteReader> {
        public TableIndex Parent;
        public TableIndex PropertyList;

        public void Read(IndexByteReader reader) {
            reader.Read(TableType.TypeDef, out this.Parent);
            reader.Read(TableType.Property, out this.PropertyList);
        }
    }
}