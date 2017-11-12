using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Signatures;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ArkeCLR.Runtime.Execution {
    public class Interpreter : IExecutionEngine {
        private readonly Action<string> logger;
        private readonly TypeSystem typeSystem;
        private readonly Stack stack;

        public Interpreter(IFileResolver fileResolver, Action<string> logger) => (this.typeSystem, this.stack, this.logger) = (new TypeSystem(fileResolver, logger), new Stack(), logger);

        public long Run(string entryAssemblyPath, IEnumerable<string> args) {
            var entryPoint = this.typeSystem.Load(entryAssemblyPath).EntryPoint;

            //TODO Get a proper signature comparison, don't forget modreqs and modopts.
            if (!entryPoint.IsStatic) throw new InvalidAssemblyException("Entry point must be static.");
            if (entryPoint.Signature.ParamCount != 0 && (entryPoint.Signature.ParamCount != 1 || entryPoint.Signature.Params[0].Type.ElementType != ElementType.SzArray || entryPoint.Signature.Params[0].Type.SzArray.Type.ElementType != ElementType.String)) throw new InvalidAssemblyException("Entry point must take no parameters or a single string[] only.");
            if (!entryPoint.Signature.RetType.IsVoid && entryPoint.Signature.RetType.Type.ElementType != ElementType.I4 && entryPoint.Signature.RetType.Type.ElementType != ElementType.U4) throw new InvalidAssemblyException("Entry point return type must be I4, U4, or void.");

            //TODO Need to actually pass args.
            if (entryPoint.Signature.ParamCount == 1)
                this.stack.Push(TypeRecord.FromObject(0));

            this.stack.Call(entryPoint);

            this.RunInterpreter();

            var ret = !entryPoint.Signature.RetType.IsVoid ? this.stack.Pop(entryPoint.Signature.RetType.Type) : default;

            this.stack.EnsureEmpty();

            return entryPoint.Signature.RetType.IsVoid ? 0L : (entryPoint.Signature.RetType.Type.ElementType == ElementType.I4 ? ret.I4 : (long)ret.U4);
        }

        private void RunInterpreter() {
            while (this.stack.Current is var frame && frame != null) {
                var inst = frame.Method.Instructions[(int)(frame.InstructionPointer++)];

                switch (inst.Op) {
                    case InstructionType.nop: break;
                    case InstructionType.@break: Debugger.Break(); break;

                    case InstructionType.ldarg_0: this.stack.Push(frame.Args[0]); break;
                    case InstructionType.ldarg_1: this.stack.Push(frame.Args[1]); break;
                    case InstructionType.ldarg_2: this.stack.Push(frame.Args[2]); break;
                    case InstructionType.ldarg_3: this.stack.Push(frame.Args[3]); break;

                    case InstructionType.ldloc_0: this.stack.Push(frame.Locals[0], frame.Method.Locals[0].Type); break;
                    case InstructionType.ldloc_1: this.stack.Push(frame.Locals[1], frame.Method.Locals[1].Type); break;
                    case InstructionType.ldloc_2: this.stack.Push(frame.Locals[2], frame.Method.Locals[2].Type); break;
                    case InstructionType.ldloc_3: this.stack.Push(frame.Locals[3], frame.Method.Locals[3].Type); break;

                    case InstructionType.stloc_0: this.stack.Pop(ref frame.Locals[0], frame.Method.Locals[0].Type); break;
                    case InstructionType.stloc_1: this.stack.Pop(ref frame.Locals[1], frame.Method.Locals[1].Type); break;
                    case InstructionType.stloc_2: this.stack.Pop(ref frame.Locals[2], frame.Method.Locals[2].Type); break;
                    case InstructionType.stloc_3: this.stack.Pop(ref frame.Locals[3], frame.Method.Locals[3].Type); break;

                    case InstructionType.ldarg_s:
                    case InstructionType.ldarga_s:
                    case InstructionType.starg_s:
                    case InstructionType.ldloc_s:
                    case InstructionType.ldloca_s:
                    case InstructionType.stloc_s:
                    case InstructionType.ldnull:
                    case InstructionType.ldc_i4_m1:
                        throw new NotImplementedException();

                    case InstructionType.ldc_i4_0: this.stack.Push(TypeRecord.FromI4(0)); break;
                    case InstructionType.ldc_i4_1: this.stack.Push(TypeRecord.FromI4(1)); break;
                    case InstructionType.ldc_i4_2: this.stack.Push(TypeRecord.FromI4(2)); break;
                    case InstructionType.ldc_i4_3: this.stack.Push(TypeRecord.FromI4(3)); break;
                    case InstructionType.ldc_i4_4: this.stack.Push(TypeRecord.FromI4(4)); break;
                    case InstructionType.ldc_i4_5: this.stack.Push(TypeRecord.FromI4(5)); break;
                    case InstructionType.ldc_i4_6: this.stack.Push(TypeRecord.FromI4(6)); break;
                    case InstructionType.ldc_i4_7: this.stack.Push(TypeRecord.FromI4(7)); break;
                    case InstructionType.ldc_i4_8: this.stack.Push(TypeRecord.FromI4(8)); break;

                    case InstructionType.ldc_i4_s:
                    case InstructionType.ldc_i4:
                    case InstructionType.ldc_i8:
                    case InstructionType.ldc_r4:
                    case InstructionType.ldc_r8:
                    case InstructionType.dup:
                    case InstructionType.pop:
                    case InstructionType.jmp:
                        throw new NotImplementedException();

                    case InstructionType.call:
                        if (inst.TableToken.Table == Streams.TableType.MemberRef) break; //TODO Need to implement calling MemberRefs
                        if (inst.TableToken.Table != Streams.TableType.MethodDef) throw new NotImplementedException();

                        this.stack.Call(this.typeSystem.FindMethod(frame.Method, inst.TableToken));

                        break;

                    case InstructionType.calli:
                        throw new NotImplementedException();

                    case InstructionType.ret: this.stack.Return(); break;

                    case InstructionType.br_s: frame.InstructionPointer = inst.BranchTarget; break;

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
                    case InstructionType.br:
                    case InstructionType.brfalse:
                    case InstructionType.brtrue:
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
                    case InstructionType.@switch:
                    case InstructionType.ldind_i1:
                    case InstructionType.ldind_u1:
                    case InstructionType.ldind_i2:
                    case InstructionType.ldind_u2:
                    case InstructionType.ldind_i4:
                    case InstructionType.ldind_u4:
                    case InstructionType.ldind_i8:
                    case InstructionType.ldind_i:
                    case InstructionType.ldind_r4:
                    case InstructionType.ldind_r8:
                    case InstructionType.ldind_ref:
                    case InstructionType.stind_ref:
                    case InstructionType.stind_i1:
                    case InstructionType.stind_i2:
                    case InstructionType.stind_i4:
                    case InstructionType.stind_i8:
                    case InstructionType.stind_r4:
                    case InstructionType.stind_r8:
                        throw new NotImplementedException();

                    case InstructionType.add: this.stack.Push(TypeRecord.Add(this.stack.Pop(), this.stack.Pop())); break;

                    case InstructionType.sub:
                    case InstructionType.mul:
                    case InstructionType.div:
                    case InstructionType.div_un:
                    case InstructionType.rem:
                    case InstructionType.rem_un:
                    case InstructionType.and:
                    case InstructionType.or:
                    case InstructionType.xor:
                    case InstructionType.shl:
                    case InstructionType.shr:
                    case InstructionType.shr_un:
                    case InstructionType.neg:
                    case InstructionType.not:
                    case InstructionType.conv_i1:
                    case InstructionType.conv_i2:
                    case InstructionType.conv_i4:
                    case InstructionType.conv_i8:
                    case InstructionType.conv_r4:
                    case InstructionType.conv_r8:
                    case InstructionType.conv_u4:
                    case InstructionType.conv_u8:
                        throw new NotImplementedException();

                    case InstructionType.callvirt: //TODO Need to dynamically invoke on the object type
                        if (inst.TableToken.Table != Streams.TableType.MethodDef) throw new NotImplementedException();

                        this.stack.Call(this.typeSystem.FindMethod(frame.Method, inst.TableToken));

                        break;

                    case InstructionType.cpobj:
                    case InstructionType.ldobj:
                    case InstructionType.ldstr:
                        throw new NotImplementedException();

                    //TODO Need to handle value types and delegates, also actually allocate something of the proper type
                    case InstructionType.newobj:
                        this.stack.Push(new TypeRecord { Tag = ElementType.Class });
                        this.stack.Call(this.typeSystem.FindMethod(frame.Method, inst.TableToken));

                        break;

                    case InstructionType.castclass:
                    case InstructionType.isinst:
                    case InstructionType.conv_r_un:
                    case InstructionType.unbox:
                    case InstructionType.@throw:
                    case InstructionType.ldfld:
                    case InstructionType.ldflda:
                    case InstructionType.stfld:
                    case InstructionType.ldsfld:
                    case InstructionType.ldsflda:
                    case InstructionType.stsfld:
                    case InstructionType.stobj:
                    case InstructionType.conv_ovf_i1_un:
                    case InstructionType.conv_ovf_i2_un:
                    case InstructionType.conv_ovf_i4_un:
                    case InstructionType.conv_ovf_i8_un:
                    case InstructionType.conv_ovf_u1_un:
                    case InstructionType.conv_ovf_u2_un:
                    case InstructionType.conv_ovf_u4_un:
                    case InstructionType.conv_ovf_u8_un:
                    case InstructionType.conv_ovf_i_un:
                    case InstructionType.conv_ovf_u_un:
                    case InstructionType.box:
                    case InstructionType.newarr:
                    case InstructionType.ldlen:
                    case InstructionType.ldelema:
                    case InstructionType.ldelem_i1:
                    case InstructionType.ldelem_u1:
                    case InstructionType.ldelem_i2:
                    case InstructionType.ldelem_u2:
                    case InstructionType.ldelem_i4:
                    case InstructionType.ldelem_u4:
                    case InstructionType.ldelem_i8:
                    case InstructionType.ldelem_i:
                    case InstructionType.ldelem_r4:
                    case InstructionType.ldelem_r8:
                    case InstructionType.ldelem_ref:
                    case InstructionType.stelem_i:
                    case InstructionType.stelem_i1:
                    case InstructionType.stelem_i2:
                    case InstructionType.stelem_i4:
                    case InstructionType.stelem_i8:
                    case InstructionType.stelem_r4:
                    case InstructionType.stelem_r8:
                    case InstructionType.stelem_ref:
                    case InstructionType.ldelem:
                    case InstructionType.stelem:
                    case InstructionType.unbox_any:
                    case InstructionType.conv_ovf_i1:
                    case InstructionType.conv_ovf_u1:
                    case InstructionType.conv_ovf_i2:
                    case InstructionType.conv_ovf_u2:
                    case InstructionType.conv_ovf_i4:
                    case InstructionType.conv_ovf_u4:
                    case InstructionType.conv_ovf_i8:
                    case InstructionType.conv_ovf_u8:
                    case InstructionType.refanyval:
                    case InstructionType.ckfinite:
                    case InstructionType.mkrefany:
                    case InstructionType.ldtoken:
                    case InstructionType.conv_u2:
                    case InstructionType.conv_u1:
                    case InstructionType.conv_i:
                    case InstructionType.conv_ovf_i:
                    case InstructionType.conv_ovf_u:
                    case InstructionType.add_ovf:
                    case InstructionType.add_ovf_un:
                    case InstructionType.mul_ovf:
                    case InstructionType.mul_ovf_un:
                    case InstructionType.sub_ovf:
                    case InstructionType.sub_ovf_un:
                    case InstructionType.endfinally:
                    case InstructionType.leave:
                    case InstructionType.leave_s:
                    case InstructionType.stind_i:
                    case InstructionType.conv_u:
                    case InstructionType.arglist:
                    case InstructionType.ceq:
                    case InstructionType.cgt:
                    case InstructionType.cgt_un:
                    case InstructionType.clt:
                    case InstructionType.clt_un:
                    case InstructionType.ldftn:
                    case InstructionType.ldvirtftn:
                    case InstructionType.ldarg:
                    case InstructionType.ldarga:
                    case InstructionType.starg:
                    case InstructionType.ldloc:
                    case InstructionType.ldloca:
                    case InstructionType.stloc:
                    case InstructionType.localloc:
                    case InstructionType.endfilter:
                    case InstructionType.unaligned_prefix:
                    case InstructionType.volatile_prefix:
                    case InstructionType.tail_prefix:
                    case InstructionType.initobj:
                    case InstructionType.constrained_prefix:
                    case InstructionType.cpblk:
                    case InstructionType.initblk:
                    case InstructionType.no_prefix:
                    case InstructionType.rethrow:
                    case InstructionType.@sizeof:
                    case InstructionType.refanytype:
                    case InstructionType.readonly_prefix:
                        throw new NotImplementedException();

                    default: throw new ExecutionEngineException("Invalid opcode.");
                }
            }
        }
    }
}
