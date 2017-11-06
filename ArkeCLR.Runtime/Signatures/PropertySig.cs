using ArkeCLR.Utilities;
using System;

namespace ArkeCLR.Runtime.Signatures {
    public class PropertySig : ICustomByteReader {
        public SignatureFlags Flags;
        public uint ParamCount;
        public CustomMod[] CustomMods;
        public Type Type;
        public Param[] Params;

        public void Read(ByteReader reader) {
            reader.ReadEnum(out this.Flags);

            if ((this.Flags & SignatureFlags.Property) == 0) throw new InvalidOperationException();

            this.ParamCount = reader.ReadCompressedU4();
            this.CustomMods = CustomMod.ReadCustomMods(reader);

            reader.ReadCustom(out this.Type);
            reader.ReadCustom(this.ParamCount, out this.Params);
        }
    }
}
