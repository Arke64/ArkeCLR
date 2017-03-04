using ArkeCLR.Runtime.Files;
using System;
using System.IO;

namespace ArkeCLR.Runtime {
    //TODO Signatures, attributes, tables, method bodies, logical layout
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
            Console.WriteLine(this.entryAssembly);

            return 0;
        }
    }
}
