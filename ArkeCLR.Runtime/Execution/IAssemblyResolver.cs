namespace ArkeCLR.Runtime.Execution {
    public interface IAssemblyResolver {
        (bool, byte[]) Resolve(AssemblyName assemblyName);
    }
}
