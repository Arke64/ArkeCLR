using ArkeCLR.Runtime.Streams;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Runtime.Files {
    public class CliFile : PeFile {
        private Dictionary<uint, MethodHeader> MethodHeaders { get; } = new Dictionary<uint, MethodHeader>();

        public CliHeader CliHeader { get; }
        public CilMetadataRootHeader CliMetadataRootHeader { get; }
        public IReadOnlyDictionary<string, CilMetadataStreamHeader> StreamHeaders { get; }

        public StringHeap StringStream { get; }
        public BlobHeap BlobStream { get; }
        public UserStringHeap UserStringsStream { get; }
        public GuidHeap GuidStream { get; }
        public TableStream TableStream { get; }

        public CliFile(byte[] data) : base(data) {
            this.CliHeader = this.ReadDataDirectory(DataDirectoryType.CliHeader).ReadStruct<CliHeader>();

            var metadata = this.ReadRva(this.CliHeader.Metadata);
            this.CliMetadataRootHeader = metadata.ReadCustom<CilMetadataRootHeader>();
            this.StreamHeaders = metadata.ReadCustom<CilMetadataStreamHeader>(this.CliMetadataRootHeader.StreamCount).ToDictionary(h => h.Name, h => h);

            if (this.CliHeader.Metadata.IsZero) throw new InvalidFileException("Not a managed assembly.");
            if (this.CliMetadataRootHeader.Signature != 0x424A5342) throw new InvalidFileException("Invalid metadata signature.");

            T read<T>() where T : Stream, new() { var res = new T(); if (this.StreamHeaders.TryGetValue(res.Name, out var val)) res.Initialize(metadata.CreateView(val.Offset, val.Size)); return res; }

            this.StringStream = read<StringHeap>();
            this.BlobStream = read<BlobHeap>();
            this.UserStringsStream = read<UserStringHeap>();
            this.GuidStream = read<GuidHeap>();
            this.TableStream = read<TableStream>();
        }

        public MethodHeader GetMethodHeader(uint rva) => this.MethodHeaders.TryGetValue(rva, out var value) ? value : (this.MethodHeaders[rva] = this.ReadCustom<MethodHeader>(rva));
    }
}
