using ArkeCLR.Utilities;
using System.Collections.Generic;

namespace ArkeCLR.Runtime.Signatures {
    public class MethodRefSig : ICustomByteReader {
        public SignatureFlags Flags;
        public uint GenParamCount;
        public uint ParamCount;
        public RetType RetType;
        public Param[] Params;
        public Param[] VarArgParams;

        public void Read(ByteReader reader) {
            reader.ReadEnum(out this.Flags);

            if ((this.Flags & SignatureFlags.Generic) != 0)
                this.GenParamCount = reader.ReadCompressedU4();

            this.ParamCount = reader.ReadCompressedU4();

            reader.ReadCustom(out this.RetType);

            var param = new List<Param>();
            var varArgParam = new List<Param>();
            var cur = param;

            for (var i = 0; i < this.ParamCount; i++) {
                if (reader.TryReadEnum(ElementType.Sentinel))
                    cur = varArgParam;

                reader.ReadCustom(out Param p);

                cur.Add(p);
            }

            this.Params = param.ToArray();
            this.VarArgParams = varArgParam.ToArray();
        }
    }
}
