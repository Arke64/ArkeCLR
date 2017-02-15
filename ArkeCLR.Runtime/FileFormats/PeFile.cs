using ArkeCLR.Runtime.PeHeaders;
using ArkeCLR.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ArkeCLR.Runtime.FileFormats {
    public class PeFile : DosFile {
        public NtHeader NtHeader { get; }
        public IReadOnlyList<RvaAndSize> DataDirectories { get; }
        public IReadOnlyList<SectionHeader> SectionHeaders { get; }
        
        public PeFile(ByteReader file) : base(file) {
            file.Seek(this.DosHeader.NewHeaderAddress, SeekOrigin.Begin);

            this.NtHeader = file.ReadStruct<NtHeader>();
            this.NtHeader.Verify();

            this.DataDirectories = file.ReadStruct<RvaAndSize>(this.NtHeader.NtSpecificFields.NumberOfDataDirectories).ToList();

            file.Seek(this.NtHeader.CoffHeader.OptionalHeaderSize - Marshal.SizeOf<StandardFields>() - Marshal.SizeOf<NtSpecificFields>() - Marshal.SizeOf<RvaAndSize>() * (int)this.NtHeader.NtSpecificFields.NumberOfDataDirectories, SeekOrigin.Current);

            this.SectionHeaders = file.ReadStruct<SectionHeader>(this.NtHeader.CoffHeader.NumberOfSections).ToList();
        }

        public RvaAndSize? GetDataDirectory(DataDirectoryType type) {
            if ((int)type >= this.NtHeader.NtSpecificFields.NumberOfDataDirectories)
                return default(RvaAndSize?);

            return this.DataDirectories[(int)type];
        }

        public uint FindFileAddressForRva(RvaAndSize rva) => this.FindFileAddressForRva(rva.Rva);

        public uint FindFileAddressForRva(uint rva) {
            var section = this.FindSectionHeaderForRva(rva);

            return rva - section.VirtualAddress + section.PointerToRawData;
        }

        public SectionHeader FindSectionHeaderForRva(uint rva) => this.SectionHeaders.FirstOrDefault(s => rva > s.VirtualAddress && rva < s.VirtualAddress + s.SizeOfRawData);
    }
}
