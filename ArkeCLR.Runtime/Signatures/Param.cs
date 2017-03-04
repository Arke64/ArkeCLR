using ArkeCLR.Utilities;
using ArkeCLR.Utilities.Extensions;

namespace ArkeCLR.Runtime.Signatures {
    public struct Param {
        public CustomMod[] CustomMods;
        public bool IsByRef;
        public bool IsTypedByRef;
        public Type Type;

        public void Read(ByteReader reader) {
            var cur = reader.ReadEnum<ElementType>();

            this.CustomMods = CustomMod.ReadCustomMods(reader, ref cur);

            if (cur == ElementType.ByRef) {
                this.IsByRef = true;

                cur = reader.ReadEnum<ElementType>();
            }
            else if (cur == ElementType.TypedByRef) {
                this.IsTypedByRef = true;

                return;
            }

            this.Type.Read(cur, reader);
        }

        public override string ToString() => $"{this.CustomMods.ToString(", ", "[", "] ", true)}{(!this.IsTypedByRef ? this.Type.ToString() : "typedref")}{(this.IsByRef ? "&" : string.Empty)}";
    }
}
