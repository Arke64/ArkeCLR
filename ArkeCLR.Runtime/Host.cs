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
            this.entryAssembly.TableStream.TypeDefs.ForEach(t => Console.WriteLine(t));
            this.entryAssembly.TableStream.TypeRefs.ForEach(t => Console.WriteLine(t));

            return 0;
        }
    }
}
