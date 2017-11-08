using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Logical;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Runtime.Execution {
    public class TypeSystem {
        private readonly List<Assembly> assemblies = new List<Assembly>();
        private readonly IAssemblyResolver assemblyResolver;
        private readonly Action<string> logger;
        private readonly Assembly entryAssembly;

        public TypeSystem(string entryAssemblyPath, IAssemblyResolver assemblyResolver, Action<string> logger) {
            (this.assemblyResolver, this.logger) = (assemblyResolver, logger);

            this.entryAssembly = this.ResolveAssembly(AssemblyName.FromFilePath(entryAssemblyPath));
        }

        private Assembly ResolveAssembly(AssemblyName name) {
            if (!this.assemblyResolver.TryResolve(name, out var data)) throw new CouldNotResolveAssemblyException(name);

            var result = new Assembly(new CliFile(new ByteReader(data)));

            this.assemblies.Add(result);

            return result;
        }

        public Method FindEntryPoint() {
            var idx = this.entryAssembly.CliFile.TableStream.ParseMetadataToken(this.entryAssembly.CliFile.CliHeader.EntryPointToken);

            return this.FindMethod(this.entryAssembly, idx);
        }

        public Method FindMethod(TableIndex index) => this.entryAssembly.Types.SelectMany(t => t.Methods).Single(m => m.Row == index.Row); //TODO Fix this, shouldn't be entryAssembly.
        public Method FindMethod(Assembly assembly, TableIndex index) => assembly.Types.SelectMany(t => t.Methods).Single(m => m.Row == index.Row);
    }
}
