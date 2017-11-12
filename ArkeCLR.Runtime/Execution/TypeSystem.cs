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

        public TypeSystem(IFileResolver fileResolver, Action<string> logger) => (this.fileResolver, this.logger) = (fileResolver, logger);

        public Assembly Load(string path) => this.Load(AssemblyName.FromFilePath(path));

        public Assembly Load(AssemblyName name) {
            if (!this.fileResolver.TryResolve(name.Name, name.HintPath, out var data)) throw new CouldNotResolveAssemblyException(name);

            var result = new Assembly(this.fileResolver, new CliFile(data));

            this.assemblies.Add(result);

            return result;
        }

        public Method FindMethod(Method callingMethod, TableToken token) => this.FindMethod(callingMethod.Type.Assembly, token);
        public Method FindMethod(Assembly callingAssembly, TableToken token) => token.Table == TableType.MethodDef ? callingAssembly.Types.SelectMany(t => t.Methods).Single(m => m.Row == token.Row) : throw new NotImplementedException();
    }
}
