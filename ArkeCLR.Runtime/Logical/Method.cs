using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Signatures;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Runtime.Tables;
using ArkeCLR.Utilities.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Runtime.Logical {
    public class Method {
        public Type Type { get; }
        public uint Row { get; }
        public string Name { get; }
        public MethodDefSig Signature { get; }
        public MethodHeader Header { get; }
        public IReadOnlyList<Instruction> Instructions { get; }

        private IEnumerable<Instruction> ReadInstructions(CliFile file, TableStreamReader reader) {
            while (reader.Position < reader.Length)
                yield return new Instruction(file, reader);
        }

        public Method(CliFile file, Type type, MethodDef def, uint row) {
            this.Type = type;
            this.Row = row;
            this.Name = file.StringStream.GetAt(def.Name);
            this.Signature = file.BlobStream.GetAt<MethodDefSig>(def.Signature);
            this.Header = file.ReadCustom<MethodHeader>(def.RVA);

            var idx = new TableIndex(this.Header.LocalVarSigTok);
            if (!idx.IsZero) {
                var foo = file.TableStream.StandAloneSigs[(int)(idx.Row) - 1];
                var bar = file.BlobStream.GetAt(foo.Signature);
            }

            this.Instructions = this.ReadInstructions(file, new TableStreamReader(file.TableStream, this.Header.Body)).ToList();
        }
    }
}
