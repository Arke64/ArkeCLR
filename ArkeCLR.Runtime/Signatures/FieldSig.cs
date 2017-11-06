using ArkeCLR.Utilities;
using System;

namespace ArkeCLR.Runtime.Signatures {
    public class FieldSig : ICustomByteReader {
        public CustomMod[] CustomMods;
        public Type Type;

        public void Read(ByteReader reader) {
            var sig = reader.ReadU1();

            if (sig != 0x06) throw new InvalidOperationException();

            this.CustomMods = CustomMod.ReadCustomMods(reader);

            reader.ReadCustom(out this.Type);
        }
    }
}
