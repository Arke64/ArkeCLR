using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Tables;
using ArkeCLR.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Runtime.Streams {
    public class TableStream : Stream {
        public override string Name => "#~";

        public CilTableStreamHeader Header { get; private set; }
        public IReadOnlyList<Module> Modules { get; private set; }
        public IReadOnlyList<TypeRef> TypeRefs { get; private set; }
        public IReadOnlyList<TypeDef> TypeDefs { get; private set; }

        public override void Initialize(ByteReader byteReader) {
            var reader = new TableStreamReader(this, byteReader);

            IReadOnlyList<T> read<T>(TableType table) where T : struct, ICustomByteReader<TableStreamReader> => reader.ReadCustom<T, TableStreamReader>(this.Header.Rows[(int)table]);

            this.Header = reader.ReadCustom<CilTableStreamHeader>();
            this.Modules = read<Module>(TableType.Module);
            this.TypeRefs = read<TypeRef>(TableType.TypeRef);
            this.TypeDefs = read<TypeDef>(TableType.TypeDef);
        }
    }

    public class TableStreamReader : ByteReader {
        private static IReadOnlyDictionary<CodedIndexType, TableType[]> CodedIndexTableMap { get; }
        private static IReadOnlyDictionary<CodedIndexType, int> CodedIndexSizeMap { get; }
        private static IReadOnlyDictionary<CodedIndexType, int> CodedIndexSizeMaskMap { get; }
        private static IReadOnlyDictionary<CodedIndexType, int> CodedIndexMaxRows { get; }

        static TableStreamReader() {
            TableStreamReader.CodedIndexTableMap = new Dictionary<CodedIndexType, TableType[]> {
                [CodedIndexType.ResolutionScope] = new[] { TableType.Module, TableType.ModuleRef, TableType.AssemblyRef, TableType.TypeRef },
                [CodedIndexType.TypeDefOrRef] = new[] { TableType.TypeDef, TableType.TypeRef, TableType.TypeSpec },
            };

            TableStreamReader.CodedIndexSizeMap = new Dictionary<CodedIndexType, int> {
                [CodedIndexType.ResolutionScope] = 2,
                [CodedIndexType.TypeDefOrRef] = 2,
            };

            TableStreamReader.CodedIndexSizeMaskMap = TableStreamReader.CodedIndexSizeMap.ToDictionary(d => d.Key, d => (1 << d.Value) - 1);
            TableStreamReader.CodedIndexMaxRows = TableStreamReader.CodedIndexSizeMap.ToDictionary(d => d.Key, d => (int)Math.Pow(2, 16 - d.Value));
        }

        private readonly TableStream stream;

        public TableStreamReader(TableStream stream, ByteReader reader) : base(reader) => this.stream = stream;

        public uint ReadHeapIndex(HeapType type) => this.stream.Header.HeapSizes[(int)type] ? this.ReadU4() : this.ReadU2();

        public TableIndex ReadCodedIndex(CodedIndexType type) {
            var idx = new TableIndex { Row = this.ReadU2() };

            idx.Table = TableStreamReader.CodedIndexTableMap[type][idx.Row & TableStreamReader.CodedIndexSizeMaskMap[type]];

            idx.Row >>= TableStreamReader.CodedIndexSizeMap[type];

            if (this.stream.Header.Rows[(int)idx.Table] >= TableStreamReader.CodedIndexMaxRows[type])
                idx.Row |= (uint)(this.ReadU2() << (16 - TableStreamReader.CodedIndexSizeMap[type]));

            return idx;
        }

        public TableIndex ReadIndex(TableType type) {
            var idx = new TableIndex { Table = type, Row = this.ReadU2() };

            if (this.stream.Header.Rows[(int)idx.Table] >= 65536)
                idx.Row |= (uint)(this.ReadU2() << 16);

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
