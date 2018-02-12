using ArkeCLR.Utilities;
using System.IO;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Signatures.Types {
    public class FnPtr : ICustomByteReader {
        public MethodDefSig MethodDefSig;
        public MethodRefSig MethodRefSig;
        public bool IsDef;

        public void Read(ByteReader reader) {
            var start = reader.Position;

            reader.ReadCustom(out this.MethodDefSig);
            this.IsDef = true;

            if ((this.MethodDefSig.Flags & SignatureFlags.VarArg) != 0) {
                this.MethodDefSig = null;
                this.IsDef = false;

                reader.Seek(start, SeekOrigin.Begin);
                reader.ReadCustom(out this.MethodRefSig);
            }
        }
    }
}
