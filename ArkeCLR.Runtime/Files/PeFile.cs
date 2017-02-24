using ArkeCLR.Utilities;
using ArkeCLR.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ArkeCLR.Runtime.Files {
    public class PeFile {
        private readonly ByteReader image;

        public DosHeader DosHeader { get; }
        public NtHeader NtHeader { get; }
        public IReadOnlyList<RvaAndSize> DataDirectories { get; }
        public IReadOnlyList<SectionHeader> SectionHeaders { get; }

        public PeFile(ByteReader image)  {
            this.image = image;

            this.image.Seek(0, SeekOrigin.Begin);
            this.DosHeader = this.image.ReadStruct<DosHeader>();

            this.image.Seek(this.DosHeader.NewHeaderAddress, SeekOrigin.Begin);
            this.NtHeader = this.image.ReadStruct<NtHeader>();
            this.DataDirectories = this.image.ReadStruct<RvaAndSize>(this.NtHeader.NtSpecificFields.NumberOfDataDirectories).ToList();

            this.image.Seek(this.NtHeader.CoffHeader.OptionalHeaderSize - 0x60 - 0x08 * (int)this.NtHeader.NtSpecificFields.NumberOfDataDirectories, SeekOrigin.Current);
            this.SectionHeaders = this.image.ReadStruct<SectionHeader>(this.NtHeader.CoffHeader.NumberOfSections).ToList();

            if (this.DosHeader.MagicNumber != 0x5A4D) throw new InvalidFileException("Invalid magic number in DOS header.");
            if (this.NtHeader.Signature != 0x00004550) throw new InvalidFileException("Invalid signature in NT header.");
            if (this.NtHeader.CoffHeader.Machine.IsInvalid()) throw new InvalidFileException("Invalid COFF machine.");
            if (this.NtHeader.StandardFields.MagicNumber != CoffMagicNumber.Pe32) throw new InvalidFileException("Invalid magic number in standard fields.");
            if (this.NtHeader.NtSpecificFields.ImageBase % 0x10000 != 0) throw new InvalidFileException("Invalid image base.");
            if (this.NtHeader.NtSpecificFields.SectionAlignment <= this.NtHeader.NtSpecificFields.FileAlignment) throw new InvalidFileException("Invalid section alignment.");
            if (this.NtHeader.NtSpecificFields.FileAlignment != 0x200) throw new InvalidFileException("Invalid file alignment.");
            if (this.NtHeader.NtSpecificFields.HeaderSize % this.NtHeader.NtSpecificFields.FileAlignment != 0) throw new InvalidFileException("Invalid header size.");
        }

        public ByteReader ReadDataDirectory(DataDirectoryType type) {
            if ((int)type >= this.NtHeader.NtSpecificFields.NumberOfDataDirectories)
                throw new ArgumentException($"Cannot find the data directory {type}.", nameof(type));

            var rva = this.DataDirectories[(int)type];

            return !rva.IsZero ? this.ReadRva(rva) : throw new ArgumentException($"The data directory {type} is not present.", nameof(type));
        }

        public ByteReader ReadRva(RvaAndSize rva) {
            var section = this.SectionHeaders.Where(s => rva.Rva > s.VirtualAddress && rva.Rva < s.VirtualAddress + s.SizeOfRawData).Single();

            return this.image.CreateView(rva.Rva - section.VirtualAddress + section.PointerToRawData, rva.Size);
        }
    }
}
