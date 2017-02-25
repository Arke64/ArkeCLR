using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Tables;
using ArkeCLR.Utilities;
using ArkeCLR.Utilities.Extensions;
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
        public IReadOnlyList<Field> Fields { get; private set; }
        public IReadOnlyList<MethodDef> MethodDefs { get; private set; }
        public IReadOnlyList<Param> Params { get; private set; }
        public IReadOnlyList<InterfaceImpl> InterfaceImpls { get; private set; }
        public IReadOnlyList<MemberRef> MemberRefs { get; private set; }
        public IReadOnlyList<Constant> Constants { get; private set; }
        public IReadOnlyList<CustomAttribute> CustomAttributes { get; private set; }
        public IReadOnlyList<FieldMarshal> FieldMarshals { get; private set; }
        public IReadOnlyList<DeclSecurity> DeclSecurities { get; private set; }
        public IReadOnlyList<ClassLayout> ClassLayouts { get; private set; }
        public IReadOnlyList<FieldLayout> FieldLayouts { get; private set; }
        public IReadOnlyList<StandAloneSig> StandAloneSigs { get; private set; }
        public IReadOnlyList<EventMap> EventMaps { get; private set; }
        public IReadOnlyList<Event> Events { get; private set; }
        public IReadOnlyList<PropertyMap> PropertyMaps { get; private set; }
        public IReadOnlyList<Property> Properties { get; private set; }
        public IReadOnlyList<MethodSemantics> MethodSemantics { get; private set; }
        public IReadOnlyList<MethodImpl> MethodImpls { get; private set; }
        public IReadOnlyList<ModuleRef> ModuleRefs { get; private set; }
        public IReadOnlyList<TypeSpec> TypeSpecs { get; private set; }
        public IReadOnlyList<ImplMap> ImplMaps { get; private set; }
        public IReadOnlyList<FieldRVA> FieldRVAs { get; private set; }
        public IReadOnlyList<Assembly> Assemblies { get; private set; }
        public IReadOnlyList<AssemblyProcessor> AssemblyProcessors { get; private set; }
        public IReadOnlyList<AssemblyOS> AssemblyOSs { get; private set; }
        public IReadOnlyList<AssemblyRef> AssemblyRefs { get; private set; }
        public IReadOnlyList<AssemblyRefProcessor> AssemblyRefProcessors { get; private set; }
        public IReadOnlyList<AssemblyRefOS> AssemblyRefOSs { get; private set; }
        public IReadOnlyList<File> Files { get; private set; }
        public IReadOnlyList<ExportedType> ExportedTypes { get; private set; }
        public IReadOnlyList<ManifestResource> ManifestResources { get; private set; }
        public IReadOnlyList<NestedClass> NestedClasses { get; private set; }
        public IReadOnlyList<GenericParam> GenericParams { get; private set; }
        public IReadOnlyList<MethodSpec> MethodSpecs { get; private set; }
        public IReadOnlyList<GenericParamConstraint> GenericParamConstraints { get; private set; }

        public override void Initialize(ByteReader byteReader) {
            var reader = new TableStreamReader(this, byteReader);

            this.Header = reader.ReadCustom<CilTableStreamHeader>();

            for (var i = 0; i < this.Header.Valid.Count; i++)
                if (this.Header.Valid[i] && ((TableType)i).IsInvalid())
                    throw new NotSupportedException($"Table index '0x{i:X}' is not supported.");

            IReadOnlyList<T> read<T>(TableType table) where T : struct, ICustomByteReader<TableStreamReader> => reader.ReadCustom<T, TableStreamReader>(this.Header.Rows[(int)table]);
            this.Modules = read<Module>(TableType.Module);
            this.TypeRefs = read<TypeRef>(TableType.TypeRef);
            this.TypeDefs = read<TypeDef>(TableType.TypeDef);
            this.Fields = read<Field>(TableType.Field);
            this.MethodDefs = read<MethodDef>(TableType.MethodDef);
            this.Params = read<Param>(TableType.Param);
            this.InterfaceImpls = read<InterfaceImpl>(TableType.InterfaceImpl);
            this.MemberRefs = read<MemberRef>(TableType.MemberRef);
            this.Constants = read<Constant>(TableType.Constant);
            this.CustomAttributes = read<CustomAttribute>(TableType.CustomAttribute);
            this.FieldMarshals = read<FieldMarshal>(TableType.FieldMarshal);
            this.DeclSecurities = read<DeclSecurity>(TableType.DeclSecurity);
            this.ClassLayouts = read<ClassLayout>(TableType.ClassLayout);
            this.FieldLayouts = read<FieldLayout>(TableType.FieldLayout);
            this.StandAloneSigs = read<StandAloneSig>(TableType.StandAloneSig);
            this.EventMaps = read<EventMap>(TableType.EventMap);
            this.Events = read<Event>(TableType.Event);
            this.PropertyMaps = read<PropertyMap>(TableType.PropertyMap);
            this.Properties = read<Property>(TableType.Property);
            this.MethodSemantics = read<MethodSemantics>(TableType.MethodSemantics);
            this.MethodImpls = read<MethodImpl>(TableType.MethodImpl);
            this.ModuleRefs = read<ModuleRef>(TableType.ModuleRef);
            this.TypeSpecs = read<TypeSpec>(TableType.TypeSpec);
            this.ImplMaps = read<ImplMap>(TableType.ImplMap);
            this.FieldRVAs = read<FieldRVA>(TableType.FieldRVA);
            this.Assemblies = read<Assembly>(TableType.Assembly);
            this.AssemblyProcessors = read<AssemblyProcessor>(TableType.AssemblyProcessor);
            this.AssemblyOSs = read<AssemblyOS>(TableType.AssemblyOS);
            this.AssemblyRefs = read<AssemblyRef>(TableType.AssemblyRef);
            this.AssemblyRefProcessors = read<AssemblyRefProcessor>(TableType.AssemblyRefProcessor);
            this.AssemblyRefOSs = read<AssemblyRefOS>(TableType.AssemblyRefOS);
            this.Files = read<File>(TableType.File);
            this.ExportedTypes = read<ExportedType>(TableType.ExportedType);
            this.ManifestResources = read<ManifestResource>(TableType.ManifestResource);
            this.NestedClasses = read<NestedClass>(TableType.NestedClass);
            this.GenericParams = read<GenericParam>(TableType.GenericParam);
            this.MethodSpecs = read<MethodSpec>(TableType.MethodSpec);
            this.GenericParamConstraints = read<GenericParamConstraint>(TableType.GenericParamConstraint);
        }
    }

    public class TableStreamReader : ByteReader {
        private static IReadOnlyDictionary<CodedIndexType, IReadOnlyList<TableType>> CodedIndexTableMap { get; }
        private static IReadOnlyDictionary<CodedIndexType, int> CodedIndexSizeMap { get; }
        private static IReadOnlyDictionary<CodedIndexType, int> CodedIndexSizeMaskMap { get; }
        private static IReadOnlyDictionary<CodedIndexType, int> CodedIndexMaxRows { get; }

        static TableStreamReader() {
            TableStreamReader.CodedIndexTableMap = new Dictionary<CodedIndexType, IReadOnlyList<TableType>> {
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

            TableStreamReader.CodedIndexSizeMap = TableStreamReader.CodedIndexTableMap.ToDictionary(d => d.Key, d => (int)Math.Ceiling(Math.Log(d.Value.Count, 2)));
            TableStreamReader.CodedIndexSizeMaskMap = TableStreamReader.CodedIndexSizeMap.ToDictionary(d => d.Key, d => (1 << d.Value) - 1);
            TableStreamReader.CodedIndexMaxRows = TableStreamReader.CodedIndexSizeMap.ToDictionary(d => d.Key, d => (int)Math.Pow(2, 16 - d.Value));
        }

        private readonly TableStream stream;

        public TableStreamReader(TableStream stream, ByteReader reader) : base(reader) => this.stream = stream;

        public uint ReadIndex(HeapType type) => this.stream.Header.HeapSizes[(int)type] ? this.ReadU4() : this.ReadU2();

        public TableIndex ReadIndex(CodedIndexType type) {
            var idx = new TableIndex { Row = this.ReadU2() };

            idx.Table = TableStreamReader.CodedIndexTableMap[type][(int)(idx.Row & TableStreamReader.CodedIndexSizeMaskMap[type])];

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

        public void Read(ref uint value, HeapType type) => value = this.ReadIndex(type);
        public void Read(ref TableIndex value, CodedIndexType type) => value = this.ReadIndex(type);
        public void Read(ref TableIndex value, TableType type) => value = this.ReadIndex(type);
    }

    public struct TableIndex {
        public TableType Table;
        public uint Row;
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
        Blob = 2
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
    }
}
