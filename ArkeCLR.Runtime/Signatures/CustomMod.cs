using ArkeCLR.Utilities;
using System.Collections.Generic;

namespace ArkeCLR.Runtime.Signatures {
    public struct CustomMod {
        public ElementType Type;
        public TypeDefOrRefOrSpecEncoded EncodedType;

        public void Read(ElementType type, ByteReader reader) {
            this.Type = type;

            this.EncodedType.Read(reader);
        }

        public static CustomMod[] ReadCustomMods(ByteReader reader, ref ElementType cur) {
            var customMods = new List<CustomMod>();

            while (cur == ElementType.CModOpt || cur == ElementType.CModReqD) {
                var mod = new CustomMod();

                mod.Read(cur, reader);

                customMods.Add(mod);

                cur = reader.ReadEnum<ElementType>();
            }

            return customMods.ToArray();
        }
    }
}
