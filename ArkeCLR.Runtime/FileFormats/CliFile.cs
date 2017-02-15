using ArkeCLR.Runtime.Metadata;
using ArkeCLR.Runtime.PeHeaders;
using ArkeCLR.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ArkeCLR.Runtime.FileFormats {
    public class CliFile : PeFile {
        public CliHeader CliHeader { get; }
        public RootHeader MetadataRootHeader { get; }
        public IReadOnlyList<StreamHeader> StreamHeaders { get; }

        public CliFile(ByteReader file) : base(file) {
            var cliRva = this.GetDataDirectory(DataDirectoryType.CliHeader) ?? throw new InvalidFileException("Not a managed assembly.");

            file.Seek(this.FindFileAddressForRva(cliRva), SeekOrigin.Begin);

            this.CliHeader = file.ReadStruct<CliHeader>();
            this.CliHeader.Verify();

            file.Seek(this.FindFileAddressForRva(this.CliHeader.Metadata), SeekOrigin.Begin);

            this.MetadataRootHeader = file.ReadCustom<RootHeader>();
            this.MetadataRootHeader.Verify();

            this.StreamHeaders = file.ReadCustom<StreamHeader>(this.MetadataRootHeader.StreamCount).ToList();
        }
    }
}
