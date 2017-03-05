using ArkeCLR.Runtime.Logical;
using System;
using System.Collections.Generic;

namespace ArkeCLR.Runtime.Execution {
    public interface IExecutionEngine {
        int Run(Assembly entryAssembly, IReadOnlyCollection<Assembly> references, Action<string> logger);
    }
}