using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Signatures.Types {
    public class Ptr : ICustomByteReader {
        public CustomMod[] CustomMods;
        public Type Type;
        public bool IsVoid;

        public void Read(ByteReader reader) {
            this.CustomMods = CustomMod.ReadCustomMods(reader);

            if (!reader.TryPeekEnum(ElementType.Void)) {
                reader.ReadCustom(out this.Type);
            }
            else {
                this.IsVoid = true;
            }
        }
    }
}
