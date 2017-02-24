using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Runtime.Files {
    public class CliFile : PeFile {
        public CliHeader CliHeader { get; }
        public CilMetadataRootHeader CliMetadataRootHeader { get; }
        public IReadOnlyDictionary<string, CilMetadataStreamHeader> StreamHeaders { get; }

        public StringStream StringStream { get; }
        public BlobStream BlobStream { get; }
        public UserStringStream UserStringsStream { get; }
        public GuidStream GuidStream { get; }
        public TableStream TableStream { get; }

        public CliFile(ByteReader image) : base(image) {
            this.CliHeader = this.ReadDataDirectory(DataDirectoryType.CliHeader).ReadStruct<CliHeader>();

            var metadata = this.ReadRva(this.CliHeader.Metadata);
            this.CliMetadataRootHeader = metadata.ReadCustom<CilMetadataRootHeader>();
            this.StreamHeaders = metadata.ReadCustom<CilMetadataStreamHeader>(this.CliMetadataRootHeader.StreamCount).ToDictionary(h => h.Name, h => h);

            if (this.CliHeader.Metadata.IsZero) throw new InvalidFileException("Not a managed assembly.");
            if (this.CliMetadataRootHeader.Signature != 0x424A5342) throw new InvalidFileException("Invalid metadata signature.");

            T read<T>(string name, Func<ByteReader, T> creator) => this.StreamHeaders.TryGetValue(name, out var val) ? creator(metadata.CreateView(val.Offset, val.Size)) : default(T);

            this.StringStream = read("#Strings", m => new StringStream(m));
            this.BlobStream = read("#Blob", m => new BlobStream(m));
            this.UserStringsStream = read("#US", m => new UserStringStream(m));
            this.GuidStream = read("#GUID", m => new GuidStream(m));
            this.TableStream = read("#~", m => new TableStream(this, m));
        }
    }
}
