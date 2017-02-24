using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;
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

            T read<T>() where T : Stream, new() { var res = new T(); if (this.StreamHeaders.TryGetValue(res.Name, out var val)) res.Initialize(metadata.CreateView(val.Offset, val.Size)); return res; }

            this.StringStream = read<StringStream>();
            this.BlobStream = read<BlobStream>();
            this.UserStringsStream = read<UserStringStream>();
            this.GuidStream = read<GuidStream>();
            this.TableStream = read<TableStream>();
        }
    }
}
