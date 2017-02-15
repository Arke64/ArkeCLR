using ArkeCLR.Runtime.Files;
using System.Threading.Tasks;

namespace ArkeCLR.Runtime {
    public class Host {
        private readonly AssemblyName entryAssemblyName;
        private readonly IAssemblyResolver assemblyResolver;

        public Host(AssemblyName entryAssemblyName, IAssemblyResolver assemblyResolver) {
            this.entryAssemblyName = entryAssemblyName;
            this.assemblyResolver = assemblyResolver;
        }

        public async Task<int> StartAsync() {
            var (found, reader) = await this.assemblyResolver.ResolveAsync(this.entryAssemblyName);

            var file = new CliFile(found ? reader : throw new CouldNotResolveAssemblyException(this.entryAssemblyName));
            
            return 0;
        }
    }
}
