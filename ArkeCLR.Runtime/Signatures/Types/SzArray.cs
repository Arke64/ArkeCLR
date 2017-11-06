using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Signatures.Types {
    public class SzArray {
        public CustomMod[] CustomMods;
        public Type Type;

        public SzArray(ByteReader reader) {
            var cur = reader.ReadEnum<ElementType>();

            this.CustomMods = CustomMod.ReadCustomMods(reader, ref cur);

            this.Type.Read(cur, reader);
        }
    }
}
