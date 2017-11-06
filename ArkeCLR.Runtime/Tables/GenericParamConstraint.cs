using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct GenericParamConstraint : ICustomByteReader<IndexByteReader> {
        public TableIndex Owner;
        public TableIndex Constraint;

        public void Read(IndexByteReader reader) {
            reader.Read(TableType.GenericParam, out this.Owner);
            reader.Read(CodedIndexType.TypeDefOrRef, out this.Constraint);
        }
    }
}