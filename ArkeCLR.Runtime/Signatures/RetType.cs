﻿using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Signatures {
    public struct RetType {
        public CustomMod[] CustomMods;
        public bool IsByRef;
        public bool IsTypedByRef;
        public bool IsVoid;
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
            else if (cur == ElementType.Void) {
                this.IsVoid = true;

                return;
            }

            this.Type.Read(cur, reader);
        }
    }
}