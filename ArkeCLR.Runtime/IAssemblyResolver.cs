namespace ArkeCLR.Runtime {
    public interface IAssemblyResolver {
        (bool, byte[]) Resolve(AssemblyName assemblyName);
    }
}
