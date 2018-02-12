using ArkeCLR.Utilities;
using System;

namespace ArkeCLR.Runtime.Streams {
    public class TokenByteReader : ByteReader {
        private readonly TableStream stream;

        public TokenByteReader(TableStream stream, ByteReader reader) : base(reader) => this.stream = stream;

        public HeapToken ReadToken(HeapType type) => type != HeapType.UserString ? new HeapToken { Heap = type, Offset = this.stream.Header.HeapSizes[(int)type] ? this.ReadU4() : this.ReadU2() } : throw new ArgumentException("#US isn't valid here.", nameof(type));
        public TableToken ReadToken(TableType type) => new TableToken { Table = type, Row = this.stream.Header.Rows[(int)type] >= 65536 ? this.ReadU4() : this.ReadU2() };

        public TableToken ReadToken(CodedIndexType type) {
            var (_, tagSize, tagMask, _) = TableStream.CodedIndexDefinitions[type];
            var isLarge = this.stream.CodedIndexSizes[type];
            var value = isLarge ? this.ReadU4() : this.ReadU2();

            return new TableToken { Table = (TableType)(value & tagMask), Row = value >> tagSize };
        }

        public void Read(HeapType type, out HeapToken value) => value = this.ReadToken(type);
        public void Read(TableType type, out TableToken value) => value = this.ReadToken(type);
        public void Read(CodedIndexType type, out TableToken value) => value = this.ReadToken(type);
    }
}
