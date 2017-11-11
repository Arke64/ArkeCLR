using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct ClassLayout : ICustomByteReader<TokenByteReader> {
        public ushort PackingSize;
        public uint ClassSize;
        public TableToken Parent;

        public void Read(TokenByteReader reader) {
            reader.Read(out this.PackingSize);
            reader.Read(out this.ClassSize);
            reader.Read(TableType.TypeDef, out this.Parent);
        }
    }
}