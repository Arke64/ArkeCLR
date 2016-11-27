using System.Threading.Tasks;

namespace ArkeCLR.Runtime {
    public class Host {
        private readonly IAssemblyResolver assemblyResolver;
        private readonly AssemblyName entryAssemblyName;

        public Host(AssemblyName entryAssemblyName, IAssemblyResolver assemblyResolver) {
            this.assemblyResolver = assemblyResolver;
            this.entryAssemblyName = entryAssemblyName;
        }

        public async Task<int> StartAsync() {
            var (entryAssemblyFound, entryAssemblyData) = await this.assemblyResolver.ResolveAsync(this.entryAssemblyName);

            if (!entryAssemblyFound)
                throw new CouldNotResolveAssemblyException(this.entryAssemblyName);

            return 0;
        }
    }
}
