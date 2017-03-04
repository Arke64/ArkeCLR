using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Logical;
using System.IO;

namespace ArkeCLR.Runtime {
    //TODO Signatures, attributes, tables, method bodies, logical layout
    public class Host {
        private readonly IAssemblyResolver assemblyResolver;
        private Assembly entryAssembly;

        public Host(IAssemblyResolver assemblyResolver) => this.assemblyResolver = assemblyResolver;

        public void Resolve(string entryAssemblyPath) {
            var name = new AssemblyName(Path.GetFileNameWithoutExtension(entryAssemblyPath), entryAssemblyPath);
            var (found, reader) = this.assemblyResolver.Resolve(name);

            if (!found)
                throw new CouldNotResolveAssemblyException(name);

            this.entryAssembly = new Assembly(new CliFile(reader));
        }

        public int Run() {
            return 0;
        }
    }
}
