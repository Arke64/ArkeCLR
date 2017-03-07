using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Streams;

namespace ArkeCLR.Runtime.Logical {
    public struct Instruction {
        public InstructionType Type;
        public ExtendedInstructionType ExtendedType;
        public TableIndex TableIndexOperand;

        public Instruction(CliFile file, IndexByteReader body) : this() {
            body.ReadEnum(out this.Type);

            if (this.Type == InstructionType.extended)
                body.ReadEnum(out this.ExtendedType);

            switch (this.Type) {
                case InstructionType.ldstr:
                case InstructionType.call:
                    body.Read(out this.TableIndexOperand); break;
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

                default:
                    noOperand = true; break;
            }

            return type + (noOperand ? string.Empty : " ") + operand;
        }
    }
}
