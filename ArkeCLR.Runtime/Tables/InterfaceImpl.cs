using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Tables {
    public struct InterfaceImpl : ICustomByteReader<TokenByteReader> {
        public TableToken Class;
        public TableToken Interface;

        public void Read(TokenByteReader reader) {
            reader.Read(TableType.TypeDef, out this.Class);
            reader.Read(CodedIndexType.TypeDefOrRef, out this.Interface);
        }
    }
}