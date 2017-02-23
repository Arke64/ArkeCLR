using ArkeCLR.Runtime.Headers;
using ArkeCLR.Utilities;
using ArkeCLR.Utilities.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ArkeCLR.Runtime {
    public class PeFile {
        public DosHeader DosHeader { get; }
        public NtHeader NtHeader { get; }
        public IReadOnlyList<RvaAndSize> DataDirectories { get; }
        public IReadOnlyList<SectionHeader> SectionHeaders { get; }
        public CliHeader CliHeader { get; }
        public CilMetadataRootHeader CliMetadataRootHeader { get; }
        public IReadOnlyList<CilMetadataStreamHeader> StreamHeaders { get; }

        public PeFile(ByteReader image)  {
            this.DosHeader = image.ReadStruct<DosHeader>();

            image.Seek(this.DosHeader.NewHeaderAddress, SeekOrigin.Begin);
            this.NtHeader = image.ReadStruct<NtHeader>();
            this.DataDirectories = image.ReadStruct<RvaAndSize>(this.NtHeader.NtSpecificFields.NumberOfDataDirectories).ToList();

            image.Seek(this.NtHeader.CoffHeader.OptionalHeaderSize - Marshal.SizeOf<StandardFields>() - Marshal.SizeOf<NtSpecificFields>() - Marshal.SizeOf<RvaAndSize>() * (int)this.NtHeader.NtSpecificFields.NumberOfDataDirectories, SeekOrigin.Current);
            this.SectionHeaders = image.ReadStruct<SectionHeader>(this.NtHeader.CoffHeader.NumberOfSections).ToList();

            var cliRva = this.FindFileAddressForDataDirectory(DataDirectoryType.CliHeader);
            image.Seek(cliRva, SeekOrigin.Begin);
            this.CliHeader = image.ReadStruct<CliHeader>();

            var metadataStart = this.FindFileAddressForRva(this.CliHeader.Metadata);
            image.Seek(metadataStart, SeekOrigin.Begin);
            this.CliMetadataRootHeader = image.ReadCustom<CilMetadataRootHeader>();
            this.StreamHeaders = image.ReadCustom<CilMetadataStreamHeader>(this.CliMetadataRootHeader.StreamCount).ToList();

            if (this.DosHeader.MagicNumber != 0x5A4D) throw new InvalidFileException(nameof(DosHeader), nameof(DosHeader.MagicNumber));
            if (this.NtHeader.Signature != 0x00004550) throw new InvalidFileException(nameof(NtHeader), nameof(NtHeader.Signature));
            if (!this.NtHeader.CoffHeader.Machine.IsValid()) throw new InvalidFileException(nameof(CoffHeader), nameof(CoffHeader.Machine));
            if (this.NtHeader.StandardFields.MagicNumber != CoffMagicNumber.Pe32) throw new InvalidFileException(nameof(StandardFields), nameof(StandardFields.MagicNumber));
            if (this.NtHeader.NtSpecificFields.ImageBase % 0x10000 != 0) throw new InvalidFileException(nameof(NtSpecificFields), nameof(NtSpecificFields.ImageBase));
            if (this.NtHeader.NtSpecificFields.SectionAlignment <= this.NtHeader.NtSpecificFields.FileAlignment) throw new InvalidFileException(nameof(NtSpecificFields), nameof(NtSpecificFields.SectionAlignment));
            if (this.NtHeader.NtSpecificFields.FileAlignment != 0x200) throw new InvalidFileException(nameof(NtSpecificFields), nameof(NtSpecificFields.FileAlignment));
            if (this.NtHeader.NtSpecificFields.HeaderSize % this.NtHeader.NtSpecificFields.FileAlignment != 0) throw new InvalidFileException(nameof(NtSpecificFields), nameof(NtSpecificFields.HeaderSize));
            if (this.CliHeader.Metadata.IsZero) throw new InvalidFileException(nameof(CliHeader), nameof(CliHeader.Metadata));
            if (this.CliMetadataRootHeader.Signature != 0x424A5342) throw new InvalidFileException(nameof(CilMetadataRootHeader), nameof(CliMetadataRootHeader.Signature));
        }

        public uint FindFileAddressForDataDirectory(DataDirectoryType type) {
            if ((int)type >= this.NtHeader.NtSpecificFields.NumberOfDataDirectories)
                throw new InvalidFileException($"Cannot find data directory {type}");

            var rva = this.DataDirectories[(int)type];

            return !rva.IsZero ? this.FindFileAddressForRva(rva) : throw new InvalidFileException($"Cannot find data directory {type}");

        }

        public uint FindFileAddressForRva(RvaAndSize rva) {
            var sections = this.SectionHeaders.Where(s => rva.Rva > s.VirtualAddress && rva.Rva < s.VirtualAddress + s.SizeOfRawData).ToList();

            if (sections.Count != 1)
                throw new InvalidFileException($"Cannot find RVA {rva}");

            return rva.Rva - sections[0].VirtualAddress + sections[0].PointerToRawData;
        }
    }
}
