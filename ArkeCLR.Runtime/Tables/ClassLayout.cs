using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct ClassLayout : ICustomByteReader<IndexByteReader> {
        public ushort PackingSize;
        public uint ClassSize;
        public TableIndex Parent;

        public void Read(IndexByteReader reader) {
            reader.Read(out this.PackingSize);
            reader.Read(out this.ClassSize);
            reader.Read(TableType.TypeDef, out this.Parent);
        }
    }
}