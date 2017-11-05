namespace ArkeCLR.Runtime.Execution {
    public interface IAssemblyResolver {
        bool Resolve(AssemblyName assemblyName, out byte[] result);
    }
}
