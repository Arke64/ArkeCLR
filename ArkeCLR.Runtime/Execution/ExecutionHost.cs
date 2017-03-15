using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Logical;
using ArkeCLR.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace ArkeCLR.Runtime.Execution {
    public class ExecutionHost {
        private readonly IExecutionEngine engine;
        private readonly IAssemblyResolver assemblyResolver;
        private readonly Action<string> logger;
        private readonly List<Assembly> assemblies = new List<Assembly>();

        public ExecutionHost(IExecutionEngine engine, IAssemblyResolver assemblyResolver, Action<string> logger) => (this.engine, this.assemblyResolver, this.logger) = (engine, assemblyResolver, logger);

        private Assembly Resolve(AssemblyName name) {
            var (found, data) = this.assemblyResolver.Resolve(name);

            if (!found)
                throw new CouldNotResolveAssemblyException(name);

            var assm = new Assembly(new CliFile(new ByteReader(data)));

            this.assemblies.Add(assm);

            return assm;
        }

        public long Run(string entryAssemblyPath) => this.engine.Run(this.Resolve(new AssemblyName(Path.GetFileNameWithoutExtension(entryAssemblyPath), entryAssemblyPath)), this.assemblies, this.logger);
    }
}
