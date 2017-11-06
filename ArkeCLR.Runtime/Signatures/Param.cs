using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Signatures {
    public class Param : ICustomByteReader {
        public CustomMod[] CustomMods;
        public bool IsByRef;
        public bool IsTypedByRef;
        public Type Type;

        public void Read(ByteReader reader) {
            this.CustomMods = CustomMod.ReadCustomMods(reader);

            if (reader.TryReadEnum(out var res, ElementType.ByRef, ElementType.TypedByRef)) {
                switch (res) {
                    case ElementType.ByRef: this.IsByRef = true; break;
                    case ElementType.TypedByRef: this.IsTypedByRef = true; return;
                }
            }

            reader.ReadCustom(out this.Type);
        }
    }
}
