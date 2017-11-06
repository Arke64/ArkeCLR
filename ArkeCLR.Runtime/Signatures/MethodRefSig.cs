using ArkeCLR.Utilities;
using System.Collections.Generic;

namespace ArkeCLR.Runtime.Signatures {
    public class MethodRefSig : ICustomByteReader {
        public bool HasThis;
        public bool ExplicitThis;
        public CallingConvention CallingConvention;
        public uint ParamCount;
        public RetType RetType;
        public Param[] Params;
        public Param[] VarArgParams;

        public void Read(ByteReader reader) {
            var first = reader.ReadU1();

            this.HasThis = (first & 0x20) != 0;
            this.ExplicitThis = (first & 0x40) != 0;

            if ((CallingConvention)(first & 0x1F) == CallingConvention.VarArg)
                this.CallingConvention = CallingConvention.VarArg;

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
