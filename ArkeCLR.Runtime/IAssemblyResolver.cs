using System.Threading.Tasks;

namespace ArkeCLR.Runtime {
    public interface IAssemblyResolver {
        Task<(bool, byte[])> ResolveAsync(AssemblyName assemblyName);
    }
}
