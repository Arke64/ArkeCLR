namespace ArkeCLR.Runtime.Execution {
    public interface IAssemblyResolver {
        bool TryResolve(AssemblyName assemblyName, out byte[] result);
    }
}
