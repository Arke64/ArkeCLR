using ArkeCLR.Utilities;
using System.IO;

namespace ArkeCLR.Runtime.Signatures.Types {
    public class FnPtr : ICustomByteReader {
        public MethodDefSig MethodDefSig;
        public MethodRefSig MethodRefSig;
        public bool IsDef;

        public void Read(ByteReader reader) {
            var start = reader.Position;

            reader.ReadCustom(out this.MethodDefSig);
            this.IsDef = true;

            if (this.MethodDefSig.CallingConvention == CallingConvention.VarArg) {
                this.MethodDefSig = null;
                reader.Seek(start, SeekOrigin.Begin);

                reader.ReadCustom(out this.MethodRefSig);
                this.IsDef = false;
            }
        }
    }
}
