using ArkeCLR.Utilities;
using System.Threading.Tasks;

namespace ArkeCLR.Runtime {
    public interface IAssemblyResolver {
        Task<(bool, ByteReader)> ResolveAsync(AssemblyName assemblyName);
    }
}
