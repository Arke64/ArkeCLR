using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;
using System.IO;
using System.Linq;

namespace ArkeCLR.Runtime.TypeSystem {
    public class Assembly {
        private readonly PeFile peFile;

        public StringStream StringsStream { get; }
        public BlobStream BlobStream { get; }
        public UserStringStream UserStringsStream { get; }
        public GuidStream GuidStream { get; }
        public TableStream TablesStream { get; }

        public Assembly(ByteReader fileData, PeFile peFile) {
            this.peFile = peFile;

            fileData.Seek(peFile.FindFileAddressForRva(peFile.CliHeader.Metadata), SeekOrigin.Begin);

            this.StringsStream = new StringStream(this.FindViewFor(fileData, "#Strings"));
            this.BlobStream = new BlobStream(this.FindViewFor(fileData, "#Blob"));
            this.UserStringsStream = new UserStringStream(this.FindViewFor(fileData, "#US"));
            this.GuidStream = new GuidStream(this.FindViewFor(fileData, "#GUID"));
            this.TablesStream = new TableStream(this, this.FindViewFor(fileData, "#~"));

            this.TablesStream.ParseTables();

            var strings = this.StringsStream.ReadAll().ToList();
            var blobs = this.BlobStream.ReadAll().ToList();
            var userStrings = this.UserStringsStream.ReadAll().ToList();
            var guids = this.GuidStream.ReadAll().ToList();
        }

        private ByteReader FindViewFor(ByteReader file, string steamName) {
            if (!this.peFile.StreamHeaders.Any(h => h.Name == steamName))
                return null;

            var header = this.peFile.StreamHeaders.Single(h => h.Name == steamName);

            return file.CreateView((uint)file.Position + header.Offset, header.Size);
        }
    }
}
