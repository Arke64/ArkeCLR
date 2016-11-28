using System;

namespace ArkeCLR.Runtime {
    public enum InvalidPeFilePart {
        MsDosStub,
        Signature,
        FileHeader,
        StandardFields,
        NtSpecificFields,
        DataDirectories
    }

    public class InvalidPeFileException : Exception {
        public InvalidPeFileException(InvalidPeFilePart invalidPeFilePart) : this(invalidPeFilePart, null) { }
        public InvalidPeFileException(InvalidPeFilePart invalidPeFilePart, Exception innerException) : base("Invalid PE file: " + invalidPeFilePart, innerException) { }
    }
}