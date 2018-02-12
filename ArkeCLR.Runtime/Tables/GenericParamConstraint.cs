using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Tables {
    public struct GenericParamConstraint : ICustomByteReader<TokenByteReader> {
        public TableToken Owner;
        public TableToken Constraint;

        public void Read(TokenByteReader reader) {
            reader.Read(TableType.GenericParam, out this.Owner);
            reader.Read(CodedIndexType.TypeDefOrRef, out this.Constraint);
        }
    }
}