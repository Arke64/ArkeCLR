using ArkeCLR.Runtime.FileFormats;
using System.IO;

namespace ArkeCLR.Runtime {
    public class Host {
        private readonly IAssemblyResolver assemblyResolver;
        private CliFile entryAssembly;

        public Host(IAssemblyResolver assemblyResolver) => this.assemblyResolver = assemblyResolver;

        public void Resolve(string entryAssemblyPath) {
            var name = new AssemblyName(Path.GetFileNameWithoutExtension(entryAssemblyPath), entryAssemblyPath);
            var (found, reader) = this.assemblyResolver.Resolve(name);

            this.entryAssembly = new CliFile(found ? reader : throw new CouldNotResolveAssemblyException(name));
        }

        public int Run() => 0;
    }
}
