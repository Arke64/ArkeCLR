using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Signatures.Types {
    public class SzArrayType {
        public CustomMod[] CustomMods;
        public Type Type;

        public SzArrayType(ByteReader reader) {
            var cur = reader.ReadEnum<ElementType>();

            this.CustomMods = CustomMod.ReadCustomMods(reader, ref cur);

            this.Type.Read(cur, reader);
        }
    }
}
