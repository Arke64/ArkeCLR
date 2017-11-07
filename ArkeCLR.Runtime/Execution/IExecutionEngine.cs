namespace ArkeCLR.Runtime.Execution {
    public interface IExecutionEngine {
        long Run(string entryAssemblyPath);
    }
}