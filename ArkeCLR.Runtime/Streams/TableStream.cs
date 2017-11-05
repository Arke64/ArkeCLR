using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Tables;
using ArkeCLR.Utilities;
using ArkeCLR.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArkeCLR.Runtime.Streams {
    public class TableStream : Stream {
        public TableStream() : base("#~") { }

        public CilTableStreamHeader Header { get; private set; }

        public TableList<Module> Modules { get; private set; }
        public TableList<TypeRef> TypeRefs { get; private set; }
        public TableList<TypeDef> TypeDefs { get; private set; }
        public TableList<Field> Fields { get; private set; }
        public TableList<MethodDef> MethodDefs { get; private set; }
        public TableList<Param> Params { get; private set; }
        public TableList<InterfaceImpl> InterfaceImpls { get; private set; }
        public TableList<MemberRef> MemberRefs { get; private set; }
        public TableList<Constant> Constants { get; private set; }
        public TableList<CustomAttribute> CustomAttributes { get; private set; }
        public TableList<FieldMarshal> FieldMarshals { get; private set; }
        public TableList<DeclSecurity> DeclSecurities { get; private set; }
        public TableList<ClassLayout> ClassLayouts { get; private set; }
        public TableList<FieldLayout> FieldLayouts { get; private set; }
        public TableList<StandAloneSig> StandAloneSigs { get; private set; }
        public TableList<EventMap> EventMaps { get; private set; }
        public TableList<Event> Events { get; private set; }
        public TableList<PropertyMap> PropertyMaps { get; private set; }
        public TableList<Property> Properties { get; private set; }
        public TableList<MethodSemantics> MethodSemantics { get; private set; }
        public TableList<MethodImpl> MethodImpls { get; private set; }
        public TableList<ModuleRef> ModuleRefs { get; private set; }
        public TableList<TypeSpec> TypeSpecs { get; private set; }
        public TableList<ImplMap> ImplMaps { get; private set; }
        public TableList<FieldRVA> FieldRVAs { get; private set; }
        public TableList<Assembly> Assemblies { get; private set; }
        public TableList<AssemblyProcessor> AssemblyProcessors { get; private set; }
        public TableList<AssemblyOS> AssemblyOSs { get; private set; }
        public TableList<AssemblyRef> AssemblyRefs { get; private set; }
        public TableList<AssemblyRefProcessor> AssemblyRefProcessors { get; private set; }
        public TableList<AssemblyRefOS> AssemblyRefOSs { get; private set; }
        public TableList<File> Files { get; private set; }
        public TableList<ExportedType> ExportedTypes { get; private set; }
        public TableList<ManifestResource> ManifestResources { get; private set; }
        public TableList<NestedClass> NestedClasses { get; private set; }
        public TableList<GenericParam> GenericParams { get; private set; }
        public TableList<MethodSpec> MethodSpecs { get; private set; }
        public TableList<GenericParamConstraint> GenericParamConstraints { get; private set; }

        public override void Initialize(ByteReader reader) {
            var indexReader = new IndexByteReader(this, reader);

            this.Header = indexReader.ReadCustom<CilTableStreamHeader>();

            for (var i = 0; i < this.Header.Valid.Count; i++)
                if (this.Header.Valid[i] && ((TableType)i).IsInvalid())
                    throw new NotSupportedException($"Table index '0x{i:X2}' is not supported.");

            TableList<T> read<T>(TableList<T> _, TableType table) where T : struct, ICustomByteReader<IndexByteReader> => new TableList<T>(this.Header.Valid[(int)table] ? indexReader.ReadCustom<T, IndexByteReader>(this.Header.Rows[(int)table]) : new T[0], table);

            this.Modules = read(this.Modules, TableType.Module);
            this.TypeRefs = read(this.TypeRefs, TableType.TypeRef);
            this.TypeDefs = read(this.TypeDefs, TableType.TypeDef);
            this.Fields = read(this.Fields, TableType.Field);
            this.MethodDefs = read(this.MethodDefs, TableType.MethodDef);
            this.Params = read(this.Params, TableType.Param);
            this.InterfaceImpls = read(this.InterfaceImpls, TableType.InterfaceImpl);
            this.MemberRefs = read(this.MemberRefs, TableType.MemberRef);
            this.Constants = read(this.Constants, TableType.Constant);
            this.CustomAttributes = read(this.CustomAttributes, TableType.CustomAttribute);
            this.FieldMarshals = read(this.FieldMarshals, TableType.FieldMarshal);
            this.DeclSecurities = read(this.DeclSecurities, TableType.DeclSecurity);
            this.ClassLayouts = read(this.ClassLayouts, TableType.ClassLayout);
            this.FieldLayouts = read(this.FieldLayouts, TableType.FieldLayout);
            this.StandAloneSigs = read(this.StandAloneSigs, TableType.StandAloneSig);
            this.EventMaps = read(this.EventMaps, TableType.EventMap);
            this.Events = read(this.Events, TableType.Event);
            this.PropertyMaps = read(this.PropertyMaps, TableType.PropertyMap);
            this.Properties = read(this.Properties, TableType.Property);
            this.MethodSemantics = read(this.MethodSemantics, TableType.MethodSemantics);
            this.MethodImpls = read(this.MethodImpls, TableType.MethodImpl);
            this.ModuleRefs = read(this.ModuleRefs, TableType.ModuleRef);
            this.TypeSpecs = read(this.TypeSpecs, TableType.TypeSpec);
            this.ImplMaps = read(this.ImplMaps, TableType.ImplMap);
            this.FieldRVAs = read(this.FieldRVAs, TableType.FieldRVA);
            this.Assemblies = read(this.Assemblies, TableType.Assembly);
            this.AssemblyProcessors = read(this.AssemblyProcessors, TableType.AssemblyProcessor);
            this.AssemblyOSs = read(this.AssemblyOSs, TableType.AssemblyOS);
            this.AssemblyRefs = read(this.AssemblyRefs, TableType.AssemblyRef);
            this.AssemblyRefProcessors = read(this.AssemblyRefProcessors, TableType.AssemblyRefProcessor);
            this.AssemblyRefOSs = read(this.AssemblyRefOSs, TableType.AssemblyRefOS);
            this.Files = read(this.Files, TableType.File);
            this.ExportedTypes = read(this.ExportedTypes, TableType.ExportedType);
            this.ManifestResources = read(this.ManifestResources, TableType.ManifestResource);
            this.NestedClasses = read(this.NestedClasses, TableType.NestedClass);
            this.GenericParams = read(this.GenericParams, TableType.GenericParam);
            this.MethodSpecs = read(this.MethodSpecs, TableType.MethodSpec);
            this.GenericParamConstraints = read(this.GenericParamConstraints, TableType.GenericParamConstraint);
        }

        public TableIndex ToTableIndex(uint value) => new TableIndex { Table = (TableType)(value >> 24), Row = value & 0xFFFFFF };

        public class TableList<T> {
            private readonly T[] list;
            private readonly TableType type;

            public uint RowCount { get; }

            public TableList(T[] source, TableType type) {
                this.list = new T[source.Length + 1];
                this.type = type;

                this.RowCount = (uint)source.Length;

                Array.Copy(source, 0, this.list, 1, source.Length);
            }

            public T Get(TableIndex index) {
                if (index.Table != this.type)
                    throw new InvalidOperationException("Wrong table index.");

                return this.list[index.Row];
            }

            public IReadOnlyCollection<TResult> ExtractRun<TParent, TResult>(TableList<TParent> parentTable, Func<TParent, uint> parentStartRowSelector, TParent parent, uint parentRow, Func<T, uint, TResult> resultSelector) {
                var end = parentRow < parentTable.RowCount ? parentStartRowSelector(parentTable.list[parentRow + 1]) : this.RowCount + 1;
                var start = parentStartRowSelector(parent);

                return this.list.Skip((int)start).Take((int)(end - start)).Select((s, i) => resultSelector(s, (uint)(start + i))).ToArray();
            }

            public void ToString(StringBuilder builder) => this.list.ForEach(t => builder.AppendLine(t.ToString()));
        }
    }

    public class IndexByteReader : ByteReader {
        private static IReadOnlyDictionary<CodedIndexType, (IReadOnlyList<TableType> tables, int size, int sizeMask, int maxRows)> CodedIndexDefinition { get; }

        static IndexByteReader() {
            var indexes = new Dictionary<CodedIndexType, IReadOnlyList<TableType>> {
                [CodedIndexType.TypeDefOrRef] = new[] { TableType.TypeDef, TableType.TypeRef, TableType.TypeSpec },
                [CodedIndexType.HasConstant] = new[] { TableType.Field, TableType.Param, TableType.Property },
                [CodedIndexType.HasCustomAttribute] = new[] { TableType.MethodDef, TableType.Field, TableType.TypeRef, TableType.TypeDef, TableType.Param, TableType.InterfaceImpl, TableType.MemberRef, TableType.Module, (TableType)0xFF /*Permission*/, TableType.Property, TableType.Event, TableType.StandAloneSig, TableType.ModuleRef, TableType.TypeSpec, TableType.Assembly, TableType.AssemblyRef, TableType.File, TableType.ExportedType, TableType.ManifestResource, TableType.GenericParam, TableType.GenericParamConstraint, TableType.MethodSpec },
                [CodedIndexType.HasFieldMarshall] = new[] { TableType.Field, TableType.Param },
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

    public struct TableIndex {
        public TableType Table;
        public uint Row;

        public bool IsZero => this.Row == 0;

        public override string ToString() => $"{this.Table.ToString()}@0x{this.Row:X8}";
    }

    public struct HeapIndex {
        public HeapType Heap;
        public uint Offset;

        public override string ToString() => $"{this.Heap.ToString()}@0x{this.Offset:X8}";
    }

    public enum CodedIndexType {
        TypeDefOrRef,
        HasConstant,
        HasCustomAttribute,
        HasFieldMarshall,
        HasDeclSecurity,
        MemberRefParent,
        HasSemantics,
        MethodDefOrRef,
        MemberForwarded,
        Implementation,
        CustomAttributeType,
        ResolutionScope,
        TypeOfMethodDef
    }

    public enum HeapType {
        String = 0,
        Guid = 1,
        Blob = 2,
        UserString = 3
    }

    public enum TableType : byte {
        Module = 0x00,
        TypeRef = 0x01,
        TypeDef = 0x02,
        Field = 0x04,
        MethodDef = 0x06,
        Param = 0x08,
        InterfaceImpl = 0x09,
        MemberRef = 0x0A,
        Constant = 0x0B,
        CustomAttribute = 0x0C,
        FieldMarshal = 0x0D,
        DeclSecurity = 0x0E,
        ClassLayout = 0x0F,
        FieldLayout = 0x10,
        StandAloneSig = 0x11,
        EventMap = 0x12,
        Event = 0x14,
        PropertyMap = 0x15,
        Property = 0x17,
        MethodSemantics = 0x18,
        MethodImpl = 0x19,
        ModuleRef = 0x1A,
        TypeSpec = 0x1B,
        ImplMap = 0x1C,
        FieldRVA = 0x1D,
        Assembly = 0x20,
        AssemblyProcessor = 0x21,
        AssemblyOS = 0x22,
        AssemblyRef = 0x23,
        AssemblyRefProcessor = 0x24,
        AssemblyRefOS = 0x25,
        File = 0x26,
        ExportedType = 0x27,
        ManifestResource = 0x28,
        NestedClass = 0x29,
        GenericParam = 0x2A,
        MethodSpec = 0x2B,
        GenericParamConstraint = 0x2C,
        UserStringHeap = 0x70
    }
}
