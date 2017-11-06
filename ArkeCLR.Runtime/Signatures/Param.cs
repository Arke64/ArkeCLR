using ArkeCLR.Utilities;
using System;

namespace ArkeCLR.Runtime.Signatures {
    public struct Param {
        public CustomMod[] CustomMods;
        public bool IsByRef;
        public bool IsTypedByRef;
        public Type Type;

        public Param(ElementType elementType) : this() {
            this.CustomMods = Array.Empty<CustomMod>();
            this.Type = new Type(elementType);
        }

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
    }
}
