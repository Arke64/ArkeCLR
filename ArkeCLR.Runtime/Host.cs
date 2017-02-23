using ArkeCLR.Runtime.TypeSystem;
using System.IO;

namespace ArkeCLR.Runtime {
    public class Host {
        private readonly IAssemblyResolver assemblyResolver;
        private Assembly entryAssembly;

        public Host(IAssemblyResolver assemblyResolver) => this.assemblyResolver = assemblyResolver;

        public void Resolve(string entryAssemblyPath) {
            var name = new AssemblyName(Path.GetFileNameWithoutExtension(entryAssemblyPath), entryAssemblyPath);
            var (found, reader) = this.assemblyResolver.Resolve(name);

            if (!found)
                throw new CouldNotResolveAssemblyException(name);

            this.entryAssembly = new Assembly(reader, new PeFile(reader));
        }

        public int Run() => 0;
    }
}
