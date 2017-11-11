using ArkeCLR.Utilities;
using System;

namespace ArkeCLR.Runtime.Streams {
    public class IndexByteReader : ByteReader {
        private readonly TableStream stream;

        public IndexByteReader(TableStream stream, ByteReader reader) : base(reader) => this.stream = stream;

        public TableIndex ReadToken() => this.stream.ParseMetadataToken(this.ReadU4());
        public HeapIndex ReadIndex(HeapType type) => type != HeapType.UserString ? new HeapIndex { Heap = type, Offset = this.stream.Header.HeapSizes[(int)type] ? this.ReadU4() : this.ReadU2() } : throw new ArgumentException("#US isn't valid here.", nameof(type));
        public TableIndex ReadIndex(TableType type) => new TableIndex { Table = type, Row = this.stream.Header.Rows[(int)type] >= 65536 ? this.ReadU4() : this.ReadU2() };

        public TableIndex ReadIndex(CodedIndexType type) {
            var def = TableStream.CodedIndexDefinitions[type];
            var isLarge = this.stream.CodedIndexSizes[type];
            var value = isLarge ? this.ReadU4() : this.ReadU2();

            return new TableIndex { Table = (TableType)(value & def.tagMask), Row = value >> def.tagSize };
        }

        public void ReadToken(out TableIndex value) => value = this.ReadToken();
        public void Read(HeapType type, out HeapIndex value) => value = this.ReadIndex(type);
        public void Read(TableType type, out TableIndex value) => value = this.ReadIndex(type);
        public void Read(CodedIndexType type, out TableIndex value) => value = this.ReadIndex(type);
    }
}
