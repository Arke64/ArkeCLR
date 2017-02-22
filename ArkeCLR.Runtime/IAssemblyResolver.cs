using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime {
    public interface IAssemblyResolver {
        (bool, ByteReader) Resolve(AssemblyName assemblyName);
    }
}
