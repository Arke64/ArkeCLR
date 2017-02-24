using System;

namespace ArkeCLR.Runtime {
    public class InvalidFileException : Exception {
        public InvalidFileException(string message) : base(message) { }
    }
}