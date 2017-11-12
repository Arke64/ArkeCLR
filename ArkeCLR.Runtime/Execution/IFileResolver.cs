namespace ArkeCLR.Runtime.Execution {
    public interface IFileResolver {
        bool TryResolve(string fileName, string hintPath, out byte[] result);
    }
}
