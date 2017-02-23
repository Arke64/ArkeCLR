using ArkeCLR.Runtime.FileFormats;
using ArkeCLR.Runtime.Headers;
using ArkeCLR.Runtime.Tables;
using ArkeCLR.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ArkeCLR.Runtime.Streams {
    public abstract class Stream<T> {
        private Dictionary<int, T> cache = new Dictionary<int, T>();

        protected readonly ByteReader reader;
        private readonly int offset;

        public Stream(ByteReader reader, int offset) => (this.reader, this.offset) = (reader, offset);

        public T GetAt(uint index) => this.GetAt((int)index);

        public T GetAt(int index) {
            if (this.cache.TryGetValue(index, out var val))
                return val;

            var actual = index - this.offset;

            if (actual < 0)
                return default(T);

            this.reader.Seek(actual, SeekOrigin.Begin);

            return this.cache[index] = this.Get();
        }

        protected abstract T Get();

        public IEnumerable<T> ReadAll() {
            while (this.reader.Position < this.reader.Length)
                yield return this.GetAt(this.reader.Position + this.offset);
        }

        protected int ReadEncodedLength() {
            var first = this.reader.ReadU1();

            if ((first & 0b1000_0000) == 0b0000_0000) return first & 0b0111_1111;
            if ((first & 0b1100_0000) == 0b1000_0000) return ((first & 0b0011_1111) << 8) + this.reader.ReadU1();

            return ((first & 0b0001_1111) << 24) + (this.reader.ReadU1() << 16) + (this.reader.ReadU1() << 8) + this.reader.ReadU1();
        }
    }

    public class StringStream : Stream<string> {
        public StringStream(ByteReader reader) : base(reader, 0) { }

        protected override string Get() => this.reader.ReadStringTerminated(Encoding.UTF8);
    }

    public class BlobStream : Stream<byte[]> {
        public BlobStream(ByteReader reader) : base(reader, 0) { }

        protected override byte[] Get() => this.reader.ReadArray<byte>(this.ReadEncodedLength());
    }

    public class UserStringStream : Stream<string> {
        public UserStringStream(ByteReader reader) : base(reader, 0) { }

        protected override string Get() {
            var length = this.ReadEncodedLength();

            if (length == 0)
                return string.Empty;

            var data = this.reader.ReadArray<byte>(length);

            //TODO Do we need to do anything? See II.24.2.4
            if (data[data.Length - 1] == 1) { }

            return Encoding.Unicode.GetString(data, 0, data.Length - 1);
        }
    }

    public class GuidStream : Stream<Guid?> {
        public GuidStream(ByteReader reader) : base(reader, 1) { }

        protected override Guid? Get() => new Guid(this.reader.ReadArray<byte>(16));
    }

    //TODO Keep heap and simple index sizes in mind. See II.24.2.6
    public class TableStream {
        private static Dictionary<CodedIndexType, TableType[]> CodedIndexTableMap;
        private static Dictionary<CodedIndexType, int> CodedIndexSizeMap;
        private static Dictionary<CodedIndexType, int> CodedIndexSizeMaskMap;
        private static Dictionary<CodedIndexType, int> CodedIndexMaxRows;

        private readonly ByteReader reader;
        private readonly CliFile parent;

        public CilTableStreamHeader Header { get; }
        public IReadOnlyList<Module> Modules { get; private set; }
        public IReadOnlyList<TypeRef> TypeRefs { get; private set; }
        public IReadOnlyList<TypeDef> TypeDefs { get; private set; }

        static TableStream() {
            TableStream.CodedIndexTableMap = new Dictionary<CodedIndexType, TableType[]> {
                [CodedIndexType.ResolutionScope] = new[] { TableType.Module, TableType.ModuleRef, TableType.AssemblyRef, TableType.TypeRef },
                [CodedIndexType.TypeDefOrRef] = new[] { TableType.TypeDef, TableType.TypeRef, TableType.TypeSpec },
            };

            TableStream.CodedIndexSizeMap = new Dictionary<CodedIndexType, int> {
                [CodedIndexType.ResolutionScope] = 2,
                [CodedIndexType.TypeDefOrRef] = 2,
            };

            TableStream.CodedIndexSizeMaskMap = TableStream.CodedIndexSizeMap.ToDictionary(d => d.Key, d => (1 << d.Value) - 1);
            TableStream.CodedIndexMaxRows = TableStream.CodedIndexSizeMap.ToDictionary(d => d.Key, d => (int)Math.Pow(2, 16 - d.Value));
        }

        public TableStream(CliFile parent, ByteReader reader) {
            this.reader = reader;
            this.parent = parent;

            this.Header = reader.ReadCustom<CilTableStreamHeader>();
        }

        public void ParseTables() {
            IReadOnlyList<T> read<T>(TableType table) where T : struct, ICustomByteReader<CliFile> => this.reader.ReadCustom<T, CliFile>(this.Header.Rows[(int)table], this.parent);

            this.Modules = read<Module>(TableType.Module);
            this.TypeRefs = read<TypeRef>(TableType.TypeRef);
            this.TypeDefs = read<TypeDef>(TableType.TypeDef);
        }

        public uint ReadHeapIndex(HeapType type) => this.Header.HeapSizes[(int)type] ? this.reader.ReadU4() : this.reader.ReadU2();

        public TableIndex ReadCodexIndex(CodedIndexType type) {
            var idx = new TableIndex { Row = this.reader.ReadU2() };

            idx.Table = TableStream.CodedIndexTableMap[type][idx.Row & TableStream.CodedIndexSizeMaskMap[type]];

            idx.Row >>= TableStream.CodedIndexSizeMap[type];

            if (this.Header.Rows[(int)idx.Table] >= TableStream.CodedIndexMaxRows[type])
                idx.Row |= (uint)(this.reader.ReadU2() << (16 - TableStream.CodedIndexSizeMap[type]));

            return idx;
        }

        public TableIndex ReadIndex(TableType type) {
            var idx = new TableIndex { Table = type, Row = this.reader.ReadU2() };

            if (this.Header.Rows[(int)idx.Table] >= 65536)
                idx.Row |= (uint)(this.reader.ReadU2() << 16);

            return idx;
        }
    }

    public struct TableIndex {
        public TableType Table;
        public uint Row;
    }

    public enum CodedIndexType {
        ResolutionScope,
        TypeDefOrRef
    }

    public enum HeapType {
        String = 0,
        Guid = 1,
        Blob = 2
    }

    public enum TableType : byte {
        Assembly = 0x20,
        AssemblyOS = 0x22,
        AssemblyProcessor = 0x21,
        AssemblyRef = 0x23,
        AssemblyRefOS = 0x25,
        AssemblyRefProcessor = 0x24,
        ClassLayout = 0x0F,
        Constant = 0x0B,
        CustomAttribute = 0x0C,
        DeclSecurity = 0x0E,
        EventMap = 0x12,
        Event = 0x14,
        ExportedType = 0x27,
        Field = 0x04,
        FieldLayout = 0x10,
        FieldMarshal = 0x0D,
        FieldRVA = 0x1D,
        File = 0x26,
        GenericParam = 0x2A,
        GenericParamConstraint = 0x2C,
        ImplMap = 0x1C,
        InterfaceImpl = 0x09,
        ManifestResource = 0x28,
        MemberRef = 0x0A,
        MethodDef = 0x06,
        MethodImpl = 0x19,
        MethodSemantics = 0x18,
        MethodSpec = 0x2B,
        Module = 0x00,
        ModuleRef = 0x1A,
        NestedClass = 0x29,
        Param = 0x08,
        Property = 0x17,
        PropertyMap = 0x15,
        StandAloneSig = 0x11,
        TypeDef = 0x02,
        TypeRef = 0x01,
        TypeSpec = 0x1B
    }
}
