using ArkeCLR.Utilities;
using System;
using System.Collections.Generic;

namespace ArkeCLR.Runtime.Streams {
    public class IndexByteReader : ByteReader {
        private static IReadOnlyDictionary<CodedIndexType, (IReadOnlyList<TableType> tables, int size, int sizeMask, int maxRows)> CodedIndexDefinition { get; }

        static IndexByteReader() {
            var indexes = new Dictionary<CodedIndexType, IReadOnlyList<TableType>> {
                [CodedIndexType.TypeDefOrRef] = new[] { TableType.TypeDef, TableType.TypeRef, TableType.TypeSpec },
                [CodedIndexType.HasConstant] = new[] { TableType.Field, TableType.Param, TableType.Property },
                [CodedIndexType.HasCustomAttribute] = new[] { TableType.MethodDef, TableType.Field, TableType.TypeRef, TableType.TypeDef, TableType.Param, TableType.InterfaceImpl, TableType.MemberRef, TableType.Module, (TableType)0xFF /*Permission*/, TableType.Property, TableType.Event, TableType.StandAloneSig, TableType.ModuleRef, TableType.TypeSpec, TableType.Assembly, TableType.AssemblyRef, TableType.File, TableType.ExportedType, TableType.ManifestResource, TableType.GenericParam, TableType.GenericParamConstraint, TableType.MethodSpec },
                [CodedIndexType.HasFieldMarshal] = new[] { TableType.Field, TableType.Param },
                [CodedIndexType.HasDeclSecurity] = new[] { TableType.TypeDef, TableType.MethodDef, TableType.Assembly },
                [CodedIndexType.MemberRefParent] = new[] { TableType.TypeDef, TableType.TypeRef, TableType.ModuleRef, TableType.MethodDef, TableType.TypeSpec },
                [CodedIndexType.HasSemantics] = new[] { TableType.Event, TableType.Property },
                [CodedIndexType.MethodDefOrRef] = new[] { TableType.MethodDef, TableType.MemberRef },
                [CodedIndexType.MemberForwarded] = new[] { TableType.Field, TableType.MethodDef },
                [CodedIndexType.Implementation] = new[] { TableType.File, TableType.AssemblyRef, TableType.ExportedType },
                [CodedIndexType.CustomAttributeType] = new[] { (TableType)0xFF /*Not defined*/, (TableType)0xFF /*Not defined*/, TableType.MethodDef, TableType.MemberRef, (TableType)0xFF /*Not defined*/ },
                [CodedIndexType.ResolutionScope] = new[] { TableType.Module, TableType.ModuleRef, TableType.AssemblyRef, TableType.TypeRef },
                [CodedIndexType.TypeOfMethodDef] = new[] { TableType.TypeDef, TableType.MethodDef },
            };

            var result = new Dictionary<CodedIndexType, (IReadOnlyList<TableType> tables, int size, int sizeMask, int maxRows)>();

            foreach (var d in indexes) {
                var size = (int)Math.Ceiling(Math.Log(d.Value.Count, 2));

                result.Add(d.Key, (d.Value, size, (1 << size) - 1, (int)Math.Pow(2, 16 - size)));
            }

            IndexByteReader.CodedIndexDefinition = result;
        }

        private readonly TableStream stream;

        public IndexByteReader(TableStream stream, ByteReader reader) : base(reader) => this.stream = stream;
        public IndexByteReader(TableStream stream, byte[] buffer) : base(buffer) => this.stream = stream;

        public TableIndex ReadIndex() => this.stream.ToTableIndex(this.ReadU4()); //TODO Is this always 4 bytes? See II.22.0
        public HeapIndex ReadIndex(HeapType type) => new HeapIndex { Heap = type, Offset = this.stream.Header.HeapSizes[(int)type] ? this.ReadU4() : this.ReadU2() };
        public TableIndex ReadIndex(TableType type) => new TableIndex { Table = type, Row = this.stream.Header.Rows[(int)type] >= 65536 ? this.ReadU4() : this.ReadU2() };

        public TableIndex ReadIndex(CodedIndexType type) {
            var def = IndexByteReader.CodedIndexDefinition[type];
            var idx = new TableIndex { Row = this.ReadU2() };

            idx.Table = def.tables[(int)(idx.Row & def.sizeMask)];

            idx.Row >>= def.size;

            if (this.stream.Header.Rows[(int)idx.Table] >= def.maxRows)
                idx.Row |= (uint)(this.ReadU2() << (16 - def.size));

            return idx;
        }

        public void Read(out TableIndex value) => value = this.ReadIndex();
        public void Read(HeapType type, out HeapIndex value) => value = this.ReadIndex(type);
        public void Read(TableType type, out TableIndex value) => value = this.ReadIndex(type);
        public void Read(CodedIndexType type, out TableIndex value) => value = this.ReadIndex(type);
    }
}
