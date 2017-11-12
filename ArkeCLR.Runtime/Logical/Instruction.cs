using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Streams;
using System.Linq;

namespace ArkeCLR.Runtime.Logical {
    public class Instruction {
        public InstructionType Type;
        public TableToken TableIndexOperand;
        public int BrTarget;

        public Instruction(MethodBody body, uint i) {
            var inst = body.Instructions[i];

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
                    var end = inst.BrTarget + body.Offsets[i + 1];

                    this.BrTarget = body.Offsets.TakeWhile(o => o != end).Count();

                    break;
            }
        }
    }
}
