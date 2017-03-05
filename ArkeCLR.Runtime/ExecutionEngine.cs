using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Logical;
using ArkeCLR.Utilities;
using System.IO;
using System;

namespace ArkeCLR.Runtime {
    public class ExecutionEngine {
        private readonly IAssemblyResolver assemblyResolver;
        private readonly Action<string> logger;

        public ExecutionEngine(IAssemblyResolver assemblyResolver, Action<string> logger) => (this.assemblyResolver, this.logger) = (assemblyResolver, logger);

        private Assembly Resolve(AssemblyName name) {
            var (found, data) = this.assemblyResolver.Resolve(name);

            if (!found)
                throw new CouldNotResolveAssemblyException(name);

            return new Assembly(new CliFile(new ByteReader(data)));
        }

        public int Run(string entryAssemblyPath) {
            var entryAssembly = this.Resolve(new AssemblyName(Path.GetFileNameWithoutExtension(entryAssemblyPath), entryAssemblyPath));

            return 0;
        }
    }
}
