using ArkeCLR.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Runtime.Pe {
    public class File {
        public Header Header { get; }
        public IReadOnlyList<SectionHeader> SectionHeaders { get; }
        public CliHeader CliHeader { get; }

        //TODO Actually expand the file in memory so RVAs don't need to be looked up.
        public File(ByteReader file) {
            this.Header = file.ReadStruct<Header>();
            this.Header.Verify();

            this.SectionHeaders = file.ReadStruct<SectionHeader>(this.Header.File.NumberOfSections).ToList();

            file.MoveTo(this.FindFileAddressForRva(this.Header.Optional.DataDirectories.CliHeader.Rva));

            this.CliHeader = file.ReadStruct<CliHeader>();
        }

        public uint FindFileAddressForRva(uint rva) {
            var section = this.FindSectionHeaderForRva(rva);

            return rva - section.VirtualAddress + section.PointerToRawData;
        }

        public SectionHeader FindSectionHeaderForRva(uint rva) => this.SectionHeaders.FirstOrDefault(s => rva > s.VirtualAddress && rva < s.VirtualAddress + s.SizeOfRawData);
    }
}
