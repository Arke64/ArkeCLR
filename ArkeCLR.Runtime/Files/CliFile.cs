using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;
using ArkeCLR.Utilities.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArkeCLR.Runtime.Files {
    public class CliFile : PeFile {
        public CliHeader CliHeader { get; }
        public CilMetadataRootHeader CliMetadataRootHeader { get; }
        public IReadOnlyDictionary<string, CilMetadataStreamHeader> StreamHeaders { get; }

        public StringHeap StringStream { get; }
        public BlobHeap BlobStream { get; }
        public UserStringHeap UserStringsStream { get; }
        public GuidHeap GuidStream { get; }
        public TableStream TableStream { get; }

        public CliFile(ByteReader image) : base(image) {
            this.CliHeader = this.ReadDataDirectory(DataDirectoryType.CliHeader).ReadStruct<CliHeader>();

            var metadata = this.ReadRva(this.CliHeader.Metadata);
            this.CliMetadataRootHeader = metadata.ReadCustom<CilMetadataRootHeader>();
            this.StreamHeaders = metadata.ReadCustom<CilMetadataStreamHeader>(this.CliMetadataRootHeader.StreamCount).ToDictionary(h => h.Name, h => h);

            if (this.CliHeader.Metadata.IsZero) throw new InvalidFileException("Not a managed assembly.");
            if (this.CliMetadataRootHeader.Signature != 0x424A5342) throw new InvalidFileException("Invalid metadata signature.");

            T read<T>() where T : Stream, new() { var res = new T(); if (this.StreamHeaders.TryGetValue(res.Name, out var val)) res.Initialize(metadata.CreateView(val.Offset, val.Size)); return res; }

            this.StringStream = read<StringHeap>();
            this.BlobStream = read<BlobHeap>();
            this.UserStringsStream = read<UserStringHeap>();
            this.GuidStream = read<GuidHeap>();
            this.TableStream = read<TableStream>();
        }

        public override string ToString() {
            var builder = new StringBuilder();

            this.StringStream.ReadAll().ForEach(s => builder.AppendLine(s));
            this.BlobStream.ReadAll().ForEach(s => builder.AppendLine(s.ToString()));
            this.UserStringsStream.ReadAll().ForEach(s => builder.AppendLine(s));
            this.GuidStream.ReadAll().ForEach(s => builder.AppendLine(s.ToString()));

            this.TableStream.Modules.ToString(builder);
            this.TableStream.TypeRefs.ToString(builder);
            this.TableStream.TypeDefs.ToString(builder);
            this.TableStream.Fields.ToString(builder);
            this.TableStream.MethodDefs.ToString(builder);
            this.TableStream.Params.ToString(builder);
            this.TableStream.InterfaceImpls.ToString(builder);
            this.TableStream.MemberRefs.ToString(builder);
            this.TableStream.Constants.ToString(builder);
            this.TableStream.CustomAttributes.ToString(builder);
            this.TableStream.FieldMarshals.ToString(builder);
            this.TableStream.DeclSecurities.ToString(builder);
            this.TableStream.ClassLayouts.ToString(builder);
            this.TableStream.FieldLayouts.ToString(builder);
            this.TableStream.StandAloneSigs.ToString(builder);
            this.TableStream.EventMaps.ToString(builder);
            this.TableStream.Events.ToString(builder);
            this.TableStream.PropertyMaps.ToString(builder);
            this.TableStream.Properties.ToString(builder);
            this.TableStream.MethodSemantics.ToString(builder);
            this.TableStream.MethodImpls.ToString(builder);
            this.TableStream.ModuleRefs.ToString(builder);
            this.TableStream.TypeSpecs.ToString(builder);
            this.TableStream.ImplMaps.ToString(builder);
            this.TableStream.FieldRVAs.ToString(builder);
            this.TableStream.Assemblies.ToString(builder);
            this.TableStream.AssemblyProcessors.ToString(builder);
            this.TableStream.AssemblyOSs.ToString(builder);
            this.TableStream.AssemblyRefs.ToString(builder);
            this.TableStream.AssemblyRefProcessors.ToString(builder);
            this.TableStream.AssemblyRefOSs.ToString(builder);
            this.TableStream.Files.ToString(builder);
            this.TableStream.ExportedTypes.ToString(builder);
            this.TableStream.ManifestResources.ToString(builder);
            this.TableStream.NestedClasses.ToString(builder);
            this.TableStream.GenericParams.ToString(builder);
            this.TableStream.MethodSpecs.ToString(builder);
            this.TableStream.GenericParamConstraints.ToString(builder);

            return builder.ToString();
        }
    }
}
