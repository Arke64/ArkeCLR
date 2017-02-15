using ArkeCLR.Runtime.Headers;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Files {
    public class DosFile {
        public DosHeader DosHeader { get; }

        public DosFile(ByteReader file) => this.DosHeader = file.ReadStruct<DosHeader>();
    }
}
