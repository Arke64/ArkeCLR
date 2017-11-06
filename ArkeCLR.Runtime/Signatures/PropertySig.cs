using ArkeCLR.Utilities;
using System;

namespace ArkeCLR.Runtime.Signatures {
    public class PropertySig : ICustomByteReader {
        public bool HasThis;
        public uint ParamCount;
        public CustomMod[] CustomMods;
        public Type Type;
        public Param[] Params;

        public void Read(ByteReader reader) {
            var first = reader.ReadU1();

            if ((first & 0x08) == 0 && (first & 0x28) == 0) throw new InvalidOperationException();

            this.HasThis = (first & 0x20) != 0;

            this.ParamCount = reader.ReadCompressedU4();
            this.CustomMods = CustomMod.ReadCustomMods(reader);

            reader.ReadCustom(out this.Type);
            reader.ReadCustom(this.ParamCount, out this.Params);
        }
    }
}
