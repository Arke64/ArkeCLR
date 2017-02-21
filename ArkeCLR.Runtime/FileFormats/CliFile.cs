using ArkeCLR.Runtime.Streams;
using ArkeCLR.Runtime.Headers;
using ArkeCLR.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ArkeCLR.Runtime.FileFormats {
    public class CliFile : PeFile {
        public CliHeader CliHeader { get; }
        public CilMetadataRootHeader MetadataRootHeader { get; }
        public IReadOnlyList<CilMetadataStreamHeader> StreamHeaders { get; }

        public StringStream StringsStream { get; }
        public BlobStream BlobStream { get; }
        public UserStringStream UserStringsStream { get; }
        public GuidStream GuidStream { get; }
        public TableStream TablesStream { get; }

        public CliFile(ByteReader file) : base(file) {
            var cliRva = this.GetDataDirectory(DataDirectoryType.CliHeader) ?? throw new InvalidFileException("Not a managed assembly.");

            file.Seek(this.FindFileAddressForRva(cliRva), SeekOrigin.Begin);

            this.CliHeader = file.ReadStruct<CliHeader>();
            this.CliHeader.Verify();

            var metadataStart = this.FindFileAddressForRva(this.CliHeader.Metadata);

            file.Seek(metadataStart, SeekOrigin.Begin);

            this.MetadataRootHeader = file.ReadCustom<CilMetadataRootHeader>();
            this.MetadataRootHeader.Verify();

            this.StreamHeaders = file.ReadCustom<CilMetadataStreamHeader>(this.MetadataRootHeader.StreamCount).ToList();

            this.StringsStream = new StringStream(this.FindViewFor(file, metadataStart, "#Strings"));
            this.BlobStream = new BlobStream(this.FindViewFor(file, metadataStart, "#Blob"));
            this.UserStringsStream = new UserStringStream(this.FindViewFor(file, metadataStart, "#US"));
            this.GuidStream = new GuidStream(this.FindViewFor(file, metadataStart, "#GUID"));
            this.TablesStream = new TableStream(this, this.FindViewFor(file, metadataStart, "#~"));

            this.TablesStream.ParseTables();

            var strings = this.StringsStream.ReadAll().ToList();
            var blobs = this.BlobStream.ReadAll().ToList();
            var userStrings = this.UserStringsStream.ReadAll().ToList();
            var guids = this.GuidStream.ReadAll().ToList();
        }

        private ByteReader FindViewFor(ByteReader file, uint metadataStart, string steamName) {
            if (!this.StreamHeaders.Any(h => h.Name == steamName))
                return null;

            var header = this.StreamHeaders.SingleOrDefault(h => h.Name == steamName);

            return file.CreateView(metadataStart + header.Offset, header.Size);
        }
    }
}
