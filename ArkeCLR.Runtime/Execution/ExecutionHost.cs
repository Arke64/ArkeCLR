using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Logical;
using ArkeCLR.Utilities;
using System;
using System.Collections.Generic;

namespace ArkeCLR.Runtime.Execution {
    public class ExecutionHost {
        private readonly IExecutionEngine engine;
        private readonly IAssemblyResolver assemblyResolver;
        private readonly Action<string> logger;

        public ExecutionHost(IExecutionEngine engine, IAssemblyResolver assemblyResolver, Action<string> logger) => (this.engine, this.assemblyResolver, this.logger) = (engine, assemblyResolver, logger);

        public long Run(string entryAssemblyPath) {
            Assembly resolve(AssemblyName name) => this.assemblyResolver.TryResolve(name, out var data) ? new Assembly(new CliFile(new ByteReader(data))) : throw new CouldNotResolveAssemblyException(name);

            var entry = resolve(AssemblyName.FromFilePath(entryAssemblyPath));
            var references = new List<Assembly>();

            return this.engine.Run(entry, references, this.logger);
        }
    }
}
