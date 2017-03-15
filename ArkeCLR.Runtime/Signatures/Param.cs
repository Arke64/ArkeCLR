using ArkeCLR.Utilities;
using ArkeCLR.Utilities.Extensions;
using System;
using System.Linq;

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

        public override string ToString() => $"{this.CustomMods.ToString(", ", "[", "] ", true)}{(!this.IsTypedByRef ? this.Type.ToString() : "typedref")}{(this.IsByRef ? "&" : string.Empty)}";

        public bool Equals(Param obj) => obj == this;
        public override bool Equals(object obj) => obj != null && this.Equals((Param)obj);

        public override int GetHashCode() {
            var hash = 17;

            hash = hash * 23 + this.IsByRef.GetHashCode();
            hash = hash * 23 + this.IsTypedByRef.GetHashCode();
            hash = hash * 23 + this.Type.GetHashCode();

            foreach (var m in this.CustomMods)
                hash = hash * 23 + m.GetHashCode();

            return hash;
        }

        public static bool operator !=(Param self, Param other) => !(self == other);
        public static bool operator ==(Param self, Param other) => self.Type == other.Type && self.IsByRef == other.IsByRef && self.IsTypedByRef == other.IsTypedByRef && self.CustomMods.SequenceEqual(other.CustomMods);
    }
}
