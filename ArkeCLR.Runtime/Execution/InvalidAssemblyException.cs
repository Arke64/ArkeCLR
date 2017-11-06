using System;

namespace ArkeCLR.Runtime.Execution {
    public class InvalidAssemblyException : Exception {
        public InvalidAssemblyException() { }
        public InvalidAssemblyException(string message) : base(message) { }
        public InvalidAssemblyException(string message, Exception innerException) : base(message, innerException) { }
    }
}
