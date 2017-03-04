using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Signatures;
using ArkeCLR.Runtime.Tables;

namespace ArkeCLR.Runtime.Logical {
    public class Method {
        public string Name { get; }
        public MethodDefSig Signature { get; }

        public Method(CliFile file, MethodDef methodDef) {
            this.Name = file.StringStream.GetAt(methodDef.Name);
            this.Signature = file.BlobStream.GetAt<MethodDefSig>(methodDef.Signature);
        }
    }
}
