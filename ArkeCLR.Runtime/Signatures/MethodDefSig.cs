using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Signatures {
    public class MethodDefSig : ICustomByteReader {
        public bool HasThis;
        public bool ExplicitThis;
        public CallingConvention CallingConvention;
        public uint GenParamCount;
        public uint ParamCount;
        public RetType RetType;
        public Param[] Params;

        public void Read(ByteReader reader) {
            var first = reader.ReadU1();

            this.HasThis = (first & 0x20) != 0;
            this.ExplicitThis = (first & 0x40) != 0;
            this.CallingConvention = (CallingConvention)(first & 0x1F);

            if (this.CallingConvention == CallingConvention.Generic)
                this.GenParamCount = reader.ReadCompressedU4();

            this.ParamCount = reader.ReadCompressedU4();

            reader.ReadCustom(out this.RetType);

            reader.ReadCustom(this.ParamCount, out this.Params);
        }
    }
}
