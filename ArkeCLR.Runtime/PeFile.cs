using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Pe {
    public static class File {
        public static void Extract(ByteReader file) {
            var header = file.ReadStruct<Header>();

            header.Verify();

            var sections = file.ReadStruct<SectionHeader>(header.File.NumberOfSections);
        }
    }
}
