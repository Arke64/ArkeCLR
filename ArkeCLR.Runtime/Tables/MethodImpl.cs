using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Tables {
    public struct MethodImpl : ICustomByteReader<TokenByteReader> {
        public TableToken Class;
        public TableToken MethodBody;
        public TableToken MethodDeclaration;

        public void Read(TokenByteReader reader) {
            reader.Read(TableType.TypeDef, out this.Class);
            reader.Read(CodedIndexType.MethodDefOrRef, out this.MethodBody);
            reader.Read(CodedIndexType.MethodDefOrRef, out this.MethodDeclaration);
        }
    }
}