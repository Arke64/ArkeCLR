using ArkeCLR.Runtime.Files;
using ArkeCLR.Utilities.Extensions;
using System;
using System.IO;

namespace ArkeCLR.Runtime {
    public class Host {
        private readonly IAssemblyResolver assemblyResolver;
        private CliFile entryAssembly;

        public Host(IAssemblyResolver assemblyResolver) => this.assemblyResolver = assemblyResolver;

        public void Resolve(string entryAssemblyPath) {
            var name = new AssemblyName(Path.GetFileNameWithoutExtension(entryAssemblyPath), entryAssemblyPath);
            var (found, reader) = this.assemblyResolver.Resolve(name);

            if (!found)
                throw new CouldNotResolveAssemblyException(name);

            this.entryAssembly = new CliFile(reader);
        }

        public int Run() {
            this.entryAssembly.StringStream.ReadAll().ForEach(s => Console.WriteLine(s));
            this.entryAssembly.BlobStream.ReadAll().ForEach(s => Console.WriteLine(s));
            this.entryAssembly.UserStringsStream.ReadAll().ForEach(s => Console.WriteLine(s));
            this.entryAssembly.GuidStream.ReadAll().ForEach(s => Console.WriteLine(s));

            this.entryAssembly.TableStream.Modules.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.TypeRefs.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.TypeDefs.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.Fields.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.MethodDefs.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.Params.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.InterfaceImpls.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.MemberRefs.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.Constants.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.CustomAttributes.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.FieldMarshals.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.DeclSecurities.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.ClassLayouts.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.FieldLayouts.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.StandAloneSigs.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.EventMaps.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.Events.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.PropertyMaps.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.Properties.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.MethodSemantics.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.MethodImpls.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.ModuleRefs.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.TypeSpecs.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.ImplMaps.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.FieldRVAs.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.Assemblies.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.AssemblyProcessors.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.AssemblyOSs.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.AssemblyRefs.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.AssemblyRefProcessors.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.AssemblyRefOSs.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.Files.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.ExportedTypes.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.ManifestResources.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.NestedClasses.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.GenericParams.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.MethodSpecs.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.GenericParamConstraints.ForEach(t => Console.WriteLine(t));

            return 0;
        }
    }
}
