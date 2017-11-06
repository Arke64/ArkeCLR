using ArkeCLR.Utilities;
using System.Collections.Generic;

namespace ArkeCLR.Runtime.Signatures {
    public class CustomMod : ICustomByteReader {
        public ElementType ElementType;
        public TypeDefOrRefOrSpecEncoded EncodedType;

        public void Read(ByteReader reader) {
            reader.ReadEnum(out this.ElementType);

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
