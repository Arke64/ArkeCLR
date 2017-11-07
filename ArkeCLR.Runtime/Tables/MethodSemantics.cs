using ArkeCLR.Runtime.Tables.Flags;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct MethodSemantics : ICustomByteReader<IndexByteReader> {
        public MethodSemanticsAttributes Semantics;
        public TableIndex Method;
        public TableIndex Association;

        public void Read(IndexByteReader reader) {
            reader.ReadEnum(out this.Semantics);
            reader.Read(TableType.MethodDef, out this.Method);
            reader.Read(CodedIndexType.HasSemantics, out this.Association);
        }
    }
}