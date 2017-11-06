using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct MethodImpl : ICustomByteReader<IndexByteReader> {
        public TableIndex Class;
        public TableIndex MethodBody;
        public TableIndex MethodDeclaration;

        public void Read(IndexByteReader reader) {
            reader.Read(TableType.TypeDef, out this.Class);
            reader.Read(CodedIndexType.MethodDefOrRef, out this.MethodBody);
            reader.Read(CodedIndexType.MethodDefOrRef, out this.MethodDeclaration);
        }
    }
}