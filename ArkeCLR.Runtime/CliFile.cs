using ArkeCLR.Runtime.Headers;
using ArkeCLR.Utilities;
using System.IO;

namespace ArkeCLR.Runtime.Files {
    public class CliFile : PeFile {
        public CliHeader CliHeader { get; }

        public CliFile(ByteReader file) : base(file) {
            var cliRva = this.GetDataDirectory(DataDirectoryType.CliHeader) ?? throw new InvalidFileException("Not a managed assembly.");

            file.Seek(this.FindFileAddressForRva(cliRva.Rva), SeekOrigin.Begin);

            this.CliHeader = file.ReadStruct<CliHeader>();
        }
    }
}
