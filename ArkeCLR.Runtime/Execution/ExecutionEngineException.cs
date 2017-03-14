using System;

namespace ArkeCLR.Runtime.Execution {
    public class ExecutionEngineException : Exception {
        public ExecutionEngineException() { }
        public ExecutionEngineException(string message) : base(message) { }
        public ExecutionEngineException(string message, Exception innerException) : base(message, innerException) { }
    }
}