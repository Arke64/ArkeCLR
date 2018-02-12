using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Signatures {
    public class RetType : ICustomByteReader {
        public CustomMod[] CustomMods;
        public bool IsByRef;
        public bool IsTypedByRef;
        public bool IsVoid;
        public Type Type;

        public void Read(ByteReader reader) {
            this.CustomMods = CustomMod.ReadCustomMods(reader);

            if (reader.TryReadEnum(out var res, ElementType.ByRef, ElementType.ByRef, ElementType.Void)) {
                switch (res) {
                    case ElementType.ByRef: this.IsByRef = true; break;
                    case ElementType.TypedByRef: this.IsTypedByRef = true; return;
                    case ElementType.Void: this.IsVoid = true; return;
                }
            }

            reader.ReadCustom(out this.Type);
        }
    }
}
