using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Signatures {
    public class MethodDefSig : ICustomByteReader {
        public SignatureFlags Flags;
        public uint GenParamCount;
        public uint ParamCount;
        public RetType RetType;
        public Param[] Params;

        public void Read(ByteReader reader) {
            reader.ReadEnum(out this.Flags);

            if ((this.Flags & SignatureFlags.Generic) != 0)
                this.GenParamCount = reader.ReadCompressedU4();

            this.ParamCount = reader.ReadCompressedU4();

            reader.ReadCustom(out this.RetType);

            reader.ReadCustom(this.ParamCount, out this.Params);
        }
    }
}
