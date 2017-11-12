using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Signatures;
using System;
using System.Collections.Generic;

namespace ArkeCLR.Runtime.Execution {
    public class Interpreter : IExecutionEngine {
        private readonly Action<string> logger;
        private readonly TypeSystem typeSystem;
        private readonly Stack stack;

        public Interpreter(IFileResolver fileResolver, Action<string> logger) => (this.typeSystem, this.stack, this.logger) = (new TypeSystem(fileResolver, logger), new Stack(), logger);

        public long Run(string entryAssemblyPath, IEnumerable<string> args) {
            var entryPoint = this.typeSystem.Load(entryAssemblyPath).EntryPoint;

            if (!entryPoint.IsStatic) throw new InvalidAssemblyException("Entry point must be static.");
            if (entryPoint.Signature.ParamCount == 0 || (entryPoint.Signature.ParamCount == 1 && entryPoint.Signature.Params[0].Type.ElementType != ElementType.SzArray)) throw new InvalidAssemblyException("Entry point must take no parameters or a single string[] only.");
            if (!entryPoint.Signature.RetType.IsVoid && entryPoint.Signature.RetType.Type.ElementType != ElementType.I4 && entryPoint.Signature.RetType.Type.ElementType != ElementType.U4) throw new InvalidAssemblyException("Entry point return type must be I4, U4, or void.");
            //TODO Need to do better comparions above

            //TODO Need to handle the optional string[] args
            this.stack.Call(entryPoint);

            if (entryPoint.Signature.ParamCount == 1)
                this.stack.Push(TypeRecord.FromObject(0));

            this.RunInterpreter();

            var returnCode = 0L;

            if (entryPoint.Signature.RetType.Type.ElementType == ElementType.I4) {
                returnCode = this.stack.Pop(ElementType.I4).I4;
            }
            else if (entryPoint.Signature.RetType.Type.ElementType == ElementType.U4) {
                returnCode = this.stack.Pop(ElementType.U4).U4;
            }

            this.stack.EnsureEmpty();

            return returnCode;
        }

        private void RunInterpreter() {
            do {
                var frame = this.stack.Current;

                while (frame.InstructionPointer < frame.Method.Instructions.Count) {
                    var inst = frame.Method.Instructions[(int)(frame.InstructionPointer++)];

                    switch (inst.Op) {
                        default: throw new NotImplementedException();

                        case InstructionType.nop: break;

                        case InstructionType.ldc_i4_0: this.stack.Push(TypeRecord.FromI4(0)); break;
                        case InstructionType.ldc_i4_1: this.stack.Push(TypeRecord.FromI4(1)); break;
                        case InstructionType.ldc_i4_2: this.stack.Push(TypeRecord.FromI4(2)); break;
                        case InstructionType.ldc_i4_3: this.stack.Push(TypeRecord.FromI4(3)); break;
                        case InstructionType.ldc_i4_4: this.stack.Push(TypeRecord.FromI4(4)); break;
                        case InstructionType.ldc_i4_5: this.stack.Push(TypeRecord.FromI4(5)); break;
                        case InstructionType.ldc_i4_6: this.stack.Push(TypeRecord.FromI4(6)); break;
                        case InstructionType.ldc_i4_7: this.stack.Push(TypeRecord.FromI4(7)); break;
                        case InstructionType.ldc_i4_8: this.stack.Push(TypeRecord.FromI4(8)); break;

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

                        case InstructionType.br_s: frame.InstructionPointer = inst.BranchTarget; break;

                        //TODO Need to handle what is on the eval stack before and after a call
                        case InstructionType.ret:
                            this.stack.Return();

                            goto end;

                        case InstructionType.call:
                        case InstructionType.callvirt: //TODO Need to dynamically invoke on the object type
                            if (inst.TableToken.Table == Streams.TableType.MemberRef) goto end;
                            this.stack.Call(this.typeSystem.FindMethod(frame.Method, inst.TableToken)); goto end;

                        //TODO Need to handle value types and delegates, also actually allocate something
                        case InstructionType.newobj: this.stack.Push(new TypeRecord { Tag = ElementType.Class }); goto case InstructionType.call;

                        case InstructionType.add: this.stack.Push(TypeRecord.Add(this.stack.Pop(), this.stack.Pop())); break;
                    }
                }

                end:
                ;
            } while (this.stack.Current != null);
        }
    }
}
