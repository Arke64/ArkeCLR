using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Streams;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Runtime.Logical {
    public class Instruction {
        public uint ByteOffset;
        public InstructionType Type;
        public TableToken TableIndexOperand;
        public int BranchInstruction;

        public Instruction(CliFile file, uint offset, MethodInstruction inst) {
            this.ByteOffset = offset;

            this.Type = inst.Op;

            switch (this.Type) {
                case InstructionType.ldstr:
                    this.TableIndexOperand = new TableToken(inst.String);
                    break;

                case InstructionType.call:
                case InstructionType.callvirt:
                case InstructionType.newobj:
                    this.TableIndexOperand = new TableToken(inst.Method);
                    break;

                case InstructionType.ldfld:
                    this.TableIndexOperand = new TableToken(inst.Field);
                    break;

                case InstructionType.br_s:
                    this.BranchInstruction = inst.BrTarget;
                    break;
            }
        }

        public void FixUp(int index, IReadOnlyList<Instruction> instructions) {
            if (this.Type == InstructionType.br_s && index < instructions.Count - 1) {
                var end = this.BranchInstruction + instructions[index + 1].ByteOffset;

                this.BranchInstruction = instructions.TakeWhile(i => i.ByteOffset != end).Count();
            }
        }
    }
}
