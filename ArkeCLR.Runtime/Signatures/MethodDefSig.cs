using ArkeCLR.Utilities;
using ArkeCLR.Utilities.Extensions;

namespace ArkeCLR.Runtime.Signatures {
    public struct MethodDefSig : ICustomByteReader {
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

            this.RetType.Read(reader);

            this.Params = new Param[this.ParamCount];
            for (var i = 0; i < this.ParamCount; i++)
                this.Params[i].Read(reader);
        }

        public override string ToString() => $"{(this.ExplicitThis ? "explicit " : string.Empty)}{(this.HasThis ? "this " : string.Empty)}{this.CallingConvention.ToString().ToLower()} {this.RetType} {this.Params.ToString(", ", "(", ")", false)}";
    }
}
