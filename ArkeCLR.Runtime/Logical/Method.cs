using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Signatures;
using ArkeCLR.Runtime.Tables;

namespace ArkeCLR.Runtime.Logical {
    public class Method {
        public string Name { get; }
        public MethodDefSig Signature { get; }
        public MethodHeader Header { get; }

        public Method(CliFile file, MethodDef def) {
            this.Name = file.StringStream.GetAt(def.Name);
            this.Signature = file.BlobStream.GetAt<MethodDefSig>(def.Signature);
            this.Header = file.ReadCustom<MethodHeader>(def.RVA);
        }
    }
}
