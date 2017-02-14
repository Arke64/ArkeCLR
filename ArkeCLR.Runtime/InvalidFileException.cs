using System;

namespace ArkeCLR.Runtime {
    public enum InvalidFilePart {
        Signature,
        FileHeader,
        StandardFields,
        NtSpecificFields,
        DataDirectories,
    }

    public class InvalidFileException : Exception {
        public InvalidFileException(string message) : base(message) { }
        public InvalidFileException(InvalidFilePart invalidFilePart) : this(invalidFilePart, null) { }
        public InvalidFileException(InvalidFilePart invalidFilePart, Exception innerException) : base($"Invalid PE file '{invalidFilePart}'", innerException) { }
    }
}