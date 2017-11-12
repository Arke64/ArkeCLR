using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Logical;
using ArkeCLR.Runtime.Streams;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Runtime.Execution {
    public class TypeSystem {
        private readonly List<Assembly> assemblies = new List<Assembly>();
        private readonly IFileResolver fileResolver;
        private readonly Action<string> logger;
        private readonly Assembly entryAssembly;

        public TypeSystem(string entryAssemblyPath, IFileResolver fileResolver, Action<string> logger) {
            (this.fileResolver, this.logger) = (fileResolver, logger);

            this.entryAssembly = this.ResolveAssembly(AssemblyName.FromFilePath(entryAssemblyPath));
        }

        private Assembly ResolveAssembly(AssemblyName name) {
            if (!this.fileResolver.TryResolve(name.Name, name.HintPath, out var data)) throw new CouldNotResolveAssemblyException(name);

            var result = new Assembly(this.fileResolver, new CliFile(data));

            this.assemblies.Add(result);

            return result;
        }

        public Method FindEntryPoint() {
            var idx = new TableToken(this.entryAssembly.CliFile.CliHeader.EntryPointToken);

            return this.FindMethod(this.entryAssembly, idx);
        }

        public Method FindMethod(TableToken token) => this.entryAssembly.Types.SelectMany(t => t.Methods).Single(m => m.Row == token.Row); //TODO Fix this, shouldn't be entryAssembly.
        public Method FindMethod(Assembly assembly, TableToken token) => assembly.Types.SelectMany(t => t.Methods).Single(m => m.Row == token.Row);
    }
}
