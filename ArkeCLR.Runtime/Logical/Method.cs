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
        public MethodDefSig Signature { get; } //TODO Remove
        public IReadOnlyList<LocalVarSig.LocalVar> Locals { get; } //TODO Add a logical type for local var.
        public IReadOnlyList<Instruction> Instructions { get; }

        public Method(CliFile file, Type type, MethodDef def, uint row) {
            this.Type = type;
            this.Row = row;
            this.Name = file.StringStream.GetAt(def.Name);
            this.Signature = file.BlobStream.GetAt<MethodDefSig>(def.Signature);

            var header = file.ReadCustom<MethodHeader>(def.RVA);
            var localVarSig = file.TableStream.ToTableIndex(header.LocalVarSigTok);

            this.Locals = !localVarSig.IsZero ? file.BlobStream.GetAt<LocalVarSig>(file.TableStream.StandAloneSigs.Get(localVarSig).Signature).Locals : new LocalVarSig.LocalVar[0];

            this.Instructions = readInstructions(new IndexByteReader(file.TableStream, header.Body)).ToList();

            this.Instructions.ForEach((idx, inst) => inst.FixUp(idx, this.Instructions));

            IEnumerable<Instruction> readInstructions(IndexByteReader reader) {
                while (reader.Position < reader.Length)
                    yield return new Instruction(file, reader);
            }
        }
    }
}
