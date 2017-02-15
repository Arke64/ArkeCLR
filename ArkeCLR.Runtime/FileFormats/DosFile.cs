using ArkeCLR.Runtime.PeHeaders;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.FileFormats {
    public class DosFile {
        public DosHeader DosHeader { get; }

        public DosFile(ByteReader file) => this.DosHeader = file.ReadStruct<DosHeader>();
    }
}
