using ArkeCLR.Utilities;
using System.Collections.Generic;

namespace ArkeCLR.Runtime.Signatures {
    public class StandAloneMethodSig : ICustomByteReader {
        public SignatureFlags Flags;
        public uint ParamCount;
        public RetType RetType;
        public Param[] Params;
        public Param[] VarArgParams;

        public void Read(ByteReader reader) {
            reader.ReadEnum(out this.Flags);

            this.ParamCount = reader.ReadCompressedU4();

            reader.ReadCustom(out this.RetType);

            var param = new List<Param>();
            var varArgParam = new List<Param>();
            var cur = param;
            var isVarArg = (this.Flags & SignatureFlags.C) != 0 || (this.Flags & SignatureFlags.VarArg) != 0;

            for (var i = 0; i < this.ParamCount; i++) {
                if (isVarArg && reader.TryReadEnum(ElementType.Sentinel))
                    cur = varArgParam;

                reader.ReadCustom(out Param p);

                cur.Add(p);
            }

            this.Params = param.ToArray();
            this.VarArgParams = varArgParam.ToArray();
        }
    }
}
