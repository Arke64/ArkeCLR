using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Tables;
using ArkeCLR.Utilities;
using ArkeCLR.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Runtime.Streams {
    public class TableStream : Stream {
        public static IReadOnlyDictionary<CodedIndexType, (IReadOnlyList<TableType> tables, int tagSize, int tagMask, int maxRows)> CodedIndexDefinitions { get; }

        static TableStream() {
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

            var result = new Dictionary<CodedIndexType, (IReadOnlyList<TableType> tables, int tagSize, int tagMask, int maxRows)>();

            foreach (var d in indexes) {
                var tagSize = (int)Math.Ceiling(Math.Log(d.Value.Count, 2));

                result.Add(d.Key, (d.Value, tagSize, (1 << tagSize) - 1, (int)Math.Pow(2, 16 - tagSize)));
            }

            TableStream.CodedIndexDefinitions = result;
        }

        public TableStream() : base("#~") { }

        public CilTableStreamHeader Header { get; private set; }
        public IReadOnlyDictionary<CodedIndexType, bool> CodedIndexSizes { get; private set; }

        //TODO Need to validate all the rules for each table.
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
            var indexReader = new TokenByteReader(this, reader);

            this.Header = indexReader.ReadCustom<CilTableStreamHeader>();
            this.CodedIndexSizes = TableStream.CodedIndexDefinitions.ToDictionary(d => d.Key, d => d.Value.tables.Any(v => v != (TableType)0xFF && this.Header.Rows[(int)v] >= d.Value.maxRows));

            for (var i = 0; i < this.Header.Valid.Count; i++)
                if (this.Header.Valid[i] && ((TableType)i).IsInvalid())
                    throw new NotSupportedException($"Table '0x{i:X2}' is not supported.");

            TableList<T> read<T>(TableList<T> _, TableType table) where T : struct, ICustomByteReader<TokenByteReader> => new TableList<T>(this.Header.Valid[(int)table] ? indexReader.ReadCustom<T, TokenByteReader>(this.Header.Rows[(int)table]) : new T[0], table);

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

            public T Get(TableToken token) => token.Table == this.type ? this.Get(token.Row) : throw new InvalidOperationException("Wrong token index.");
            public T Get(uint row) => this.list[row];

            public IReadOnlyCollection<TResult> ExtractRun<TParent, TResult>(TableList<TParent> parentTable, Func<TParent, uint> parentStartRowSelector, TParent parent, uint parentRow, Func<T, uint, TResult> resultSelector) {
                var end = parentRow < parentTable.RowCount ? parentStartRowSelector(parentTable.list[parentRow + 1]) : this.RowCount + 1;
                var start = parentStartRowSelector(parent);

                return this.list.Skip((int)start).Take((int)(end - start)).Select((s, i) => resultSelector(s, (uint)(start + i))).ToArray();
            }
        }
    }

    public struct TableToken {
        public TableType Table;
        public uint Row;

        public bool IsZero => this.Row == 0;

        public TableToken(uint value) : this((TableType)(value >> 24), value & 0xFFFFFF) { }
        public TableToken(TableType table, uint row) => (this.Table, this.Row) = (table, row);
    }

    public enum CodedIndexType {
        TypeDefOrRef,
        HasConstant,
        HasCustomAttribute,
        HasFieldMarshal,
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
