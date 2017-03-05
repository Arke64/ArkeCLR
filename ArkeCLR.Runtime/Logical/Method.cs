using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Signatures;
using ArkeCLR.Runtime.Tables;
using ArkeCLR.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Runtime.Logical {
    public class Method {
        public string Name { get; }
        public MethodDefSig Signature { get; }
        public MethodHeader Header { get; }
        public IReadOnlyList<Instruction> Instructions { get; }

        private IEnumerable<Instruction> ReadInstructions(ByteReader reader) {
            while (reader.Position < reader.Length)
                yield return reader.ReadCustom<Instruction>();
        }

        public Method(CliFile file, MethodDef def) {
            this.Name = file.StringStream.GetAt(def.Name);
            this.Signature = file.BlobStream.GetAt<MethodDefSig>(def.Signature);
            this.Header = file.ReadCustom<MethodHeader>(def.RVA);

            this.Instructions = this.ReadInstructions(new ByteReader(this.Header.Body)).ToList();
        }
    }
}
