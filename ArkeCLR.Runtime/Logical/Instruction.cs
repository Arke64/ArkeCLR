using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Streams;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Runtime.Logical {
    public class Instruction {
        public int ByteOffset;
        public InstructionType Type;
        public ExtendedInstructionType ExtendedType;
        public TableIndex TableIndexOperand;
        public int BranchInstruction;

        public Instruction(CliFile file, IndexByteReader body) {
            this.ByteOffset = body.Position;

            body.ReadEnum(out this.Type);

            if (this.Type == InstructionType.extended)
                body.ReadEnum(out this.ExtendedType);

            switch (this.Type) {
                case InstructionType.ldstr:
                case InstructionType.call:
                    body.Read(out this.TableIndexOperand); break;

                case InstructionType.br_s: this.BranchInstruction = body.ReadI1(); break;
            }
        }

        public override string ToString() {
            var type = this.Type != InstructionType.extended ? this.Type.ToString() : this.ExtendedType.ToString();
            var operand = string.Empty;
            var noOperand = false;

            switch (this.Type) {
                case InstructionType.ldstr:
                case InstructionType.call:
                    operand = this.TableIndexOperand.ToString(); break;

                case InstructionType.br_s:
                    operand = this.BranchInstruction.ToString(); break;

                default:
                    noOperand = true; break;
            }

            return type + (noOperand ? string.Empty : " ") + operand;
        }

        public void FixUp(int index, IReadOnlyList<Instruction> instructions) {
            if (this.Type == InstructionType.br_s) {
                var end = this.BranchInstruction + instructions[index + 1].ByteOffset;

                this.BranchInstruction = instructions.TakeWhile(i => i.ByteOffset != end).Count();
            }
        }
    }
}
