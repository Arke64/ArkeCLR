using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Signatures;
using ArkeCLR.Runtime.Tables;
using ArkeCLR.Utilities.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Runtime.Logical {
    public class Method {
        public Type Type { get; }
        public uint Row { get; }
        public string Name { get; }
        public MethodDefSig Signature { get; } //TODO Remove
        public IReadOnlyList<LocalVarSig.LocalVar> Locals { get; } //TODO Add a logical type for local var.
        public IReadOnlyList<Instruction> Instructions { get; }

        public Method(CliFile file, Type type, MethodDef def, uint row) {
            this.Type = type;
            this.Row = row;
            this.Name = file.StringStream.GetAt(def.Name);
            this.Signature = file.BlobStream.GetAt<MethodDefSig>(def.Signature);

            if (def.RVA != 0) {
                var header = file.GetMethodHeader(def.RVA);
                var localVarSig = file.TableStream.ParseMetadataToken(header.LocalVarSigTok);

                this.Locals = !localVarSig.IsZero ? file.BlobStream.GetAt<LocalVarSig>(file.TableStream.StandAloneSigs.Get(localVarSig).Signature).Locals : new LocalVarSig.LocalVar[0];

                this.Instructions = readInstructions(header.Body).ToList();

                this.Instructions.ForEach((idx, inst) => inst.FixUp(idx, this.Instructions));
            }
            else {
                this.Locals = new LocalVarSig.LocalVar[0];
                this.Instructions = new List<Instruction>();
            }

            IEnumerable<Instruction> readInstructions(MethodBody reader) {
                for (var i = 0; i < reader.Instructions.Length; i++)
                    yield return new Instruction(file, reader.Offsets[i], reader.Instructions[i]);
            }
        }
    }
}
