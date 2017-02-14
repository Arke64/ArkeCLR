using System;

namespace ArkeCLR.Runtime {
    public class CouldNotResolveAssemblyException : Exception {
        public AssemblyName AssemblyName { get; }

        public CouldNotResolveAssemblyException(AssemblyName assemblyName) : this(assemblyName, null) { }
        public CouldNotResolveAssemblyException(AssemblyName assemblyName, Exception innerException) : base($"Could not resolve assembly '{assemblyName.FullName}'", innerException) => this.AssemblyName = assemblyName;
    }
}
