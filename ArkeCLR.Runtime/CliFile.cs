using ArkeCLR.Runtime.Headers;
using ArkeCLR.Runtime.Pe;
using ArkeCLR.Utilities;
using System.IO;

namespace ArkeCLR.Runtime {
    public class CliFile : PeFile {
        public CliHeader CliHeader { get; }

        public CliFile(ByteReader file) : base(file) {
            var (hasCli, cliRva) = this.GetDataDirectory(DataDirectoryType.CliHeader);

            if (!hasCli)
                throw new InvalidFileException("Not a managed assembly.");

            file.Seek(this.FindFileAddressForRva(cliRva.Rva), SeekOrigin.Begin);

            this.CliHeader = file.ReadStruct<CliHeader>();
        }
    }
}
