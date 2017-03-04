using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Logical;
using ArkeCLR.Utilities;
using System.IO;

namespace ArkeCLR.Runtime {
    //TODO Signatures, attributes, tables, method bodies, logical layout
    public class Host {
        private readonly IAssemblyResolver assemblyResolver;

        private Assembly Resolve(AssemblyName name) {
            var (found, data) = this.assemblyResolver.Resolve(name);

            if (!found)
                throw new CouldNotResolveAssemblyException(name);

            return new Assembly(new CliFile(new ByteReader(data)));
        }

        public Host(IAssemblyResolver assemblyResolver) => this.assemblyResolver = assemblyResolver;

        public int Run(string entryAssemblyPath) {
            var entryAssembly = this.Resolve(new AssemblyName(Path.GetFileNameWithoutExtension(entryAssemblyPath), entryAssemblyPath));

            return 0;
        }
    }
}
