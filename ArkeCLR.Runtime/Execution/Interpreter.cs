using ArkeCLR.Runtime.Logical;
using ArkeCLR.Runtime.Signatures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Runtime.Execution {
    public class CallFrame {
        public Method Method;
        public int InstructionPointer;
        public TypeRecord[] Args;
        public TypeRecord[] Locals;

        public CallFrame(Method method, int instructionPointer) {
            this.Method = method;
            this.InstructionPointer = instructionPointer;
            this.Args = new TypeRecord[method.Signature.ParamCount + (method.Signature.HasThis ? 1 : 0)];
            this.Locals = new TypeRecord[method.LocalVariablesSignature.Count];

            if (method.Signature.HasThis)
                this.Args[0].Tag = ElementType.Class; //TODO Need to record actual class, also valuetypes

            for (int i = 0, j = method.Signature.HasThis ? 1 : 0; i < method.Signature.ParamCount; i++, j++)
                this.Args[j].Tag = method.Signature.Params[i].Type.ElementType;

            for (var i = 0; i < method.LocalVariablesSignature.Count; i++)
                this.Locals[i].Tag = method.LocalVariablesSignature.Locals[i].Type.ElementType;
        }
    }

    public class Interpreter : IExecutionEngine {
        private readonly Stack<TypeRecord> evaluationStack = new Stack<TypeRecord>();
        private readonly Stack<CallFrame> callStack = new Stack<CallFrame>();

        public long Run(Assembly entryAssembly, IReadOnlyCollection<Assembly> references, Action<string> logger) {
            if (entryAssembly.EntryPoint.Signature.HasThis || entryAssembly.EntryPoint.Signature.ExplicitThis) throw new InvalidAssemblyException("Entry point must be static.");
            if (entryAssembly.EntryPoint.Signature.ParamCount == 0 || (entryAssembly.EntryPoint.Signature.ParamCount == 1 && entryAssembly.EntryPoint.Signature.Params[0] != new Param(ElementType.SzArray))) throw new InvalidAssemblyException("Entry point must take no parameters or a single string[] only.");
            if (!entryAssembly.EntryPoint.Signature.RetType.IsVoid && entryAssembly.EntryPoint.Signature.RetType.Type != new Signatures.Type(ElementType.I4) && entryAssembly.EntryPoint.Signature.RetType.Type != new Signatures.Type(ElementType.U4)) throw new InvalidAssemblyException("Entry point return type must be I4, U4, or void.");

            //TODO Need to handle the optional string[] args
            this.callStack.Push(new CallFrame(entryAssembly.EntryPoint, 0));

            if (entryAssembly.EntryPoint.Signature.ParamCount == 1)
                this.evaluationStack.Push(TypeRecord.FromObject(0));

            this.RunInterpreter();

            var returnCode = 0L;

            if (entryAssembly.EntryPoint.Signature.RetType.Type == new Signatures.Type(ElementType.I4)) {
                returnCode = this.Pop(ElementType.I4).I4;
            }
            else if (entryAssembly.EntryPoint.Signature.RetType.Type == new Signatures.Type(ElementType.U4)) {
                returnCode = this.Pop(ElementType.U4).U4;
            }

            if (this.evaluationStack.Any())
                throw new ExecutionEngineException("Expected empty stack.");

            return returnCode;
        }

        private void RunInterpreter() {
            do {
                var frame = this.callStack.Peek();

                while (frame.InstructionPointer < frame.Method.Instructions.Count) {
                    var inst = frame.Method.Instructions[frame.InstructionPointer++];

                    switch (inst.Type) {
                        default: throw new NotImplementedException();

                        case InstructionType.nop: break;

                        case InstructionType.ldc_i4_0: this.Push(TypeRecord.FromI4(0)); break;
                        case InstructionType.ldc_i4_1: this.Push(TypeRecord.FromI4(1)); break;
                        case InstructionType.ldc_i4_2: this.Push(TypeRecord.FromI4(2)); break;
                        case InstructionType.ldc_i4_3: this.Push(TypeRecord.FromI4(3)); break;
                        case InstructionType.ldc_i4_4: this.Push(TypeRecord.FromI4(4)); break;
                        case InstructionType.ldc_i4_5: this.Push(TypeRecord.FromI4(5)); break;
                        case InstructionType.ldc_i4_6: this.Push(TypeRecord.FromI4(6)); break;
                        case InstructionType.ldc_i4_7: this.Push(TypeRecord.FromI4(7)); break;
                        case InstructionType.ldc_i4_8: this.Push(TypeRecord.FromI4(8)); break;

                        case InstructionType.ldarg_0: this.Push(frame.Args[0]); break;
                        case InstructionType.ldarg_1: this.Push(frame.Args[1]); break;
                        case InstructionType.ldarg_2: this.Push(frame.Args[2]); break;
                        case InstructionType.ldarg_3: this.Push(frame.Args[3]); break;

                        case InstructionType.ldloc_0: this.Push(frame.Locals[0], frame.Method.LocalVariablesSignature.Locals[0].Type); break;
                        case InstructionType.ldloc_1: this.Push(frame.Locals[1], frame.Method.LocalVariablesSignature.Locals[1].Type); break;
                        case InstructionType.ldloc_2: this.Push(frame.Locals[2], frame.Method.LocalVariablesSignature.Locals[2].Type); break;
                        case InstructionType.ldloc_3: this.Push(frame.Locals[3], frame.Method.LocalVariablesSignature.Locals[3].Type); break;

                        case InstructionType.stloc_0: this.Pop(ref frame.Locals[0], frame.Method.LocalVariablesSignature.Locals[0].Type); break;
                        case InstructionType.stloc_1: this.Pop(ref frame.Locals[1], frame.Method.LocalVariablesSignature.Locals[1].Type); break;
                        case InstructionType.stloc_2: this.Pop(ref frame.Locals[2], frame.Method.LocalVariablesSignature.Locals[2].Type); break;
                        case InstructionType.stloc_3: this.Pop(ref frame.Locals[3], frame.Method.LocalVariablesSignature.Locals[3].Type); break;

                        case InstructionType.br_s: frame.InstructionPointer = inst.BranchInstruction; break;

                        //TODO Need to handle what is on the eval stack before and after a call
                        case InstructionType.ret:
                            var ret = default(TypeRecord);

                            if (!frame.Method.Signature.RetType.IsVoid)
                                ret = this.Pop();

                            for (var i = 0; i < frame.Method.Signature.ParamCount + (frame.Method.Signature.HasThis ? 1 : 0); i++)
                                this.Pop();

                            this.callStack.Pop();

                            if (!frame.Method.Signature.RetType.IsVoid)
                                this.Push(ret);

                            goto end;

                        case InstructionType.call:
                        case InstructionType.callvirt: //TODO Need to dynamically invoke on the object type
                            if (inst.TableIndexOperand.Table == Streams.TableType.MemberRef) goto end;
                            this.callStack.Push(new CallFrame(frame.Method.Type.Assembly.FindMethod(inst.TableIndexOperand), 0)); goto end;

                        //TODO Need to handle value types and delegates, also actually allocate something
                        case InstructionType.newobj: this.Push(new TypeRecord { Tag = ElementType.Class }); goto case InstructionType.call;

                        case InstructionType.add: this.Push(TypeRecord.Add(this.Pop(), this.Pop())); break;

                        case InstructionType.extended:
                            switch (inst.ExtendedType) {
                                default: throw new NotImplementedException();
                            }

                            break;
                    }
                }

                end:
                ;
            } while (this.callStack.Any());
        }

        private TypeRecord Pop() => this.evaluationStack.Pop();
        private TypeRecord Pop(Signatures.Type expected) => this.Pop(expected.ElementType);

        private void Pop(ref TypeRecord destination) => destination = this.Pop();
        private void Pop(ref TypeRecord destination, Signatures.Type expected) => destination = this.Pop(expected);

        private TypeRecord Pop(ElementType expected) {
            var result = this.Pop();

            if (result.Tag != expected)
                throw new ExecutionEngineException($"Popped {result.Tag} but expected {expected}.");

            return result;
        }

        private void Push(TypeRecord value) => this.evaluationStack.Push(value);
        private void Push(TypeRecord value, Signatures.Type expected) => this.Push(value, expected.ElementType);

        private void Push(TypeRecord value, ElementType expected) {
            if (value.Tag != expected)
                throw new ExecutionEngineException($"Pushed {value.Tag} but expected {expected}.");

            this.Push(value);
        }
    }
}
