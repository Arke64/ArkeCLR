using System.Collections.Generic;

namespace ArkeCLR.Runtime.Execution {
    public interface IExecutionEngine {
        long Run(string entryAssemblyPath, IEnumerable<string> args);
    }
}