using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct InterfaceImpl : ICustomByteReader<IndexByteReader> {
        public TableIndex Class;
        public TableIndex Interface;

        public void Read(IndexByteReader reader) {
            reader.Read(TableType.TypeDef, out this.Class);
            reader.Read(CodedIndexType.TypeDefOrRef, out this.Interface);
        }
    }
}