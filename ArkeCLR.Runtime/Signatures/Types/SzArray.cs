using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Signatures.Types {
    public class SzArray : ICustomByteReader {
        public CustomMod[] CustomMods;
        public Type Type;

        public void Read(ByteReader reader) {
            this.CustomMods = CustomMod.ReadCustomMods(reader);

            reader.ReadCustom(out this.Type);
        }
    }
}
