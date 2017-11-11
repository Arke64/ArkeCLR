using ArkeCLR.Runtime.Tables.Flags;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct MethodSemantics : ICustomByteReader<TokenByteReader> {
        public MethodSemanticsAttributes Semantics;
        public TableToken Method;
        public TableToken Association;

        public void Read(TokenByteReader reader) {
            reader.ReadEnum(out this.Semantics);
            reader.Read(TableType.MethodDef, out this.Method);
            reader.Read(CodedIndexType.HasSemantics, out this.Association);
        }
    }
}