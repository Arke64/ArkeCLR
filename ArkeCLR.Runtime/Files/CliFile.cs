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

        public StringStream StringStream { get; }
        public BlobStream BlobStream { get; }
        public UserStringStream UserStringsStream { get; }
        public GuidStream GuidStream { get; }
        public TableStream TableStream { get; }

        public CliFile(ByteReader image) : base(image) {
            this.CliHeader = this.ReadDataDirectory(DataDirectoryType.CliHeader).ReadStruct<CliHeader>();

            var metadata = this.ReadRva(this.CliHeader.Metadata);
            this.CliMetadataRootHeader = metadata.ReadCustom<CilMetadataRootHeader>();
            this.StreamHeaders = metadata.ReadCustom<CilMetadataStreamHeader>(this.CliMetadataRootHeader.StreamCount).ToDictionary(h => h.Name, h => h);

            if (this.CliHeader.Metadata.IsZero) throw new InvalidFileException("Not a managed assembly.");
            if (this.CliMetadataRootHeader.Signature != 0x424A5342) throw new InvalidFileException("Invalid metadata signature.");

            T read<T>() where T : Stream, new() { var res = new T(); if (this.StreamHeaders.TryGetValue(res.Name, out var val)) res.Initialize(metadata.CreateView(val.Offset, val.Size)); return res; }

            this.StringStream = read<StringStream>();
            this.BlobStream = read<BlobStream>();
            this.UserStringsStream = read<UserStringStream>();
            this.GuidStream = read<GuidStream>();
            this.TableStream = read<TableStream>();
        }

        public override string ToString() {
            var builder = new StringBuilder();

            this.StringStream.ReadAll().ForEach(s => builder.AppendLine(s));
            this.BlobStream.ReadAll().ForEach(s => builder.AppendLine(s.ToString()));
            this.UserStringsStream.ReadAll().ForEach(s => builder.AppendLine(s));
            this.GuidStream.ReadAll().ForEach(s => builder.AppendLine(s.ToString()));

            this.TableStream.Modules.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.TypeRefs.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.TypeDefs.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.Fields.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.MethodDefs.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.Params.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.InterfaceImpls.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.MemberRefs.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.Constants.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.CustomAttributes.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.FieldMarshals.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.DeclSecurities.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.ClassLayouts.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.FieldLayouts.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.StandAloneSigs.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.EventMaps.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.Events.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.PropertyMaps.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.Properties.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.MethodSemantics.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.MethodImpls.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.ModuleRefs.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.TypeSpecs.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.ImplMaps.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.FieldRVAs.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.Assemblies.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.AssemblyProcessors.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.AssemblyOSs.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.AssemblyRefs.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.AssemblyRefProcessors.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.AssemblyRefOSs.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.Files.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.ExportedTypes.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.ManifestResources.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.NestedClasses.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.GenericParams.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.MethodSpecs.ForEach(t => builder.AppendLine(t.ToString()));
            this.TableStream.GenericParamConstraints.ForEach(t => builder.AppendLine(t.ToString()));

            return builder.ToString();
        }
    }
}
