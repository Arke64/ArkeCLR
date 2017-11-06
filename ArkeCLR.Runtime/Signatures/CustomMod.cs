using ArkeCLR.Utilities;
using System.Collections.Generic;

namespace ArkeCLR.Runtime.Signatures {
    public class CustomMod : ICustomByteReader {
        public bool IsRequired;
        public TypeDefOrRefOrSpecEncoded EncodedType;

        public void Read(ByteReader reader) {
            this.IsRequired = reader.ReadEnum<ElementType>() == ElementType.CModReqD;

            this.EncodedType.Read(reader);
        }

        public static CustomMod[] ReadCustomMods(ByteReader reader) {
            var mods = new List<CustomMod>();

            while (reader.TryPeekEnum(out var res, ElementType.CModOpt, ElementType.CModReqD))
                mods.Add(reader.ReadCustom<CustomMod>());

            return mods.ToArray();
        }
    }
}
