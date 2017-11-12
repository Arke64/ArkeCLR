using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Runtime.Logical {
    public class Instruction {
        public InstructionType Op { get; }
        public TableToken TableToken { get; }
        public uint BranchTarget { get; }
        public IReadOnlyList<uint> SwitchTable { get; }
        public ushort Var { get; }
        public int I4 { get; }
        public long I8 { get; }
        public float R4 { get; }
        public double R8 { get; }

        private static uint ConvertRelativeByteOffsetToIndex(MethodBody body, uint index, int byteOffset) {
            var end = byteOffset + body.Offsets[index + 1];

            return (uint)body.Offsets.TakeWhile(o => o != end).Count();
        }

        public Instruction(MethodBody body, uint index) {
            var inst = body.Instructions[index];

            this.Op = inst.Op;

            switch (this.Op) {
                case InstructionType.beq:
                case InstructionType.bge:
                case InstructionType.bgt:
                case InstructionType.ble:
                case InstructionType.blt:
                case InstructionType.bne_un:
                case InstructionType.bge_un:
                case InstructionType.bgt_un:
                case InstructionType.ble_un:
                case InstructionType.blt_un:
                case InstructionType.br:
                case InstructionType.brfalse:
                case InstructionType.brtrue:
                case InstructionType.leave:
                    this.BranchTarget = Instruction.ConvertRelativeByteOffsetToIndex(body, index, inst.BrTarget);
                    break;

                case InstructionType.ldfld:
                case InstructionType.ldflda:
                case InstructionType.stfld:
                case InstructionType.ldsfld:
                case InstructionType.ldsflda:
                case InstructionType.stsfld:
                    this.TableToken = new TableToken(inst.Field);
                    break;

                case InstructionType.ldc_i4:
                    this.I4 = inst.I;
                    break;

                case InstructionType.ldc_i8:
                    this.I8 = inst.I8;
                    break;

                case InstructionType.callvirt:
                case InstructionType.newobj:
                case InstructionType.ldftn:
                case InstructionType.ldvirtftn:
                case InstructionType.jmp:
                case InstructionType.call:
                    this.TableToken = new TableToken(inst.Method);
                    break;

                case InstructionType.ldc_r8:
                    this.R8 = inst.R;
                    break;

                case InstructionType.calli:
                    this.TableToken = new TableToken(inst.Sig);
                    break;

                case InstructionType.ldstr:
                    this.TableToken = new TableToken(inst.String);
                    break;

                case InstructionType.@switch:
                    this.SwitchTable = inst.SwitchTable.ToList(s => Instruction.ConvertRelativeByteOffsetToIndex(body, index, s));
                    break;

                case InstructionType.ldtoken:
                    this.TableToken = new TableToken(inst.Tok);
                    break;

                case InstructionType.initobj:
                case InstructionType.cpobj:
                case InstructionType.ldobj:
                case InstructionType.castclass:
                case InstructionType.isinst:
                case InstructionType.newarr:
                case InstructionType.ldelema:
                case InstructionType.ldelem:
                case InstructionType.stelem:
                case InstructionType.unbox_any:
                case InstructionType.constrained_prefix:
                case InstructionType.@sizeof:
                case InstructionType.unbox:
                case InstructionType.stobj:
                case InstructionType.box:
                case InstructionType.refanyval:
                case InstructionType.mkrefany:
                    this.TableToken = new TableToken(inst.Type);
                    break;

                case InstructionType.ldarg:
                case InstructionType.ldarga:
                case InstructionType.starg:
                case InstructionType.ldloc:
                case InstructionType.ldloca:
                case InstructionType.stloc:
                    this.Var = inst.Var;
                    break;

                case InstructionType.br_s:
                case InstructionType.brfalse_s:
                case InstructionType.brtrue_s:
                case InstructionType.beq_s:
                case InstructionType.bge_s:
                case InstructionType.bgt_s:
                case InstructionType.ble_s:
                case InstructionType.blt_s:
                case InstructionType.bne_un_s:
                case InstructionType.bge_un_s:
                case InstructionType.bgt_un_s:
                case InstructionType.ble_un_s:
                case InstructionType.blt_un_s:
                case InstructionType.leave_s:
                    goto case InstructionType.br;

                case InstructionType.ldc_i4_s:
                case InstructionType.unaligned_prefix:
                    this.I4 = inst.ShortI;
                    break;

                case InstructionType.ldc_r4:
                    this.R4 = inst.ShortR;
                    break;

                case InstructionType.ldarg_s:
                case InstructionType.ldarga_s:
                case InstructionType.starg_s:
                case InstructionType.ldloc_s:
                case InstructionType.ldloca_s:
                case InstructionType.stloc_s:
                    this.Var = inst.ShortVar;
                    break;
            }
        }
    }
}
