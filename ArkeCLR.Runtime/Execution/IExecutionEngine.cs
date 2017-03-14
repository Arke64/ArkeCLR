using ArkeCLR.Runtime.Logical;
using System;
using System.Collections.Generic;

namespace ArkeCLR.Runtime.Execution {
    public interface IExecutionEngine {
        long Run(Assembly entryAssembly, IReadOnlyCollection<Assembly> references, Action<string> logger);
    }
}