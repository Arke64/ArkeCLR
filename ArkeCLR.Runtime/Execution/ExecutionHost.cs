using System.Collections.Generic;

namespace ArkeCLR.Runtime.Execution {
    public class ExecutionHost {
        private readonly IExecutionEngine engine;

        public ExecutionHost(IExecutionEngine engine) => this.engine = engine;

        public long Run(string entryAssemblyPath, IEnumerable<string> args) => this.engine.Run(entryAssemblyPath, args);
    }
}
