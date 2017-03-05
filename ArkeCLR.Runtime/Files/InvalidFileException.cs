using System;

namespace ArkeCLR.Runtime.Files {
    public class InvalidFileException : Exception {
        public InvalidFileException(string message) : base(message) { }
    }
}