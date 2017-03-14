using ArkeCLR.Runtime.Logical;
using ArkeCLR.Runtime.Signatures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Runtime.Execution {
    public class CallFrame {
        public Method Method;
        public int InstructionPointer;
        public TypeRecord[] Locals;

        public CallFrame(Method method, int instructionPointer) {
            this.Method = method;
            this.InstructionPointer = instructionPointer;
            this.Locals = new TypeRecord[method.LocalVariablesSignature.Count];
        }
    }

    public class Interpreter : IExecutionEngine {
        private readonly Stack<TypeRecord> evaluationStack = new Stack<TypeRecord>();
        private readonly Stack<CallFrame> callStack = new Stack<CallFrame>();

        public long Run(Assembly entryAssembly, IReadOnlyCollection<Assembly> references, Action<string> logger) {
            //TODO Need to handle the optional string[] args
            this.callStack.Push(new CallFrame(entryAssembly.EntryPoint, 0));

            this.RunInterpreter();

            var returnCode = 0L;

            if (entryAssembly.EntryPoint.Signature.RetType.Type == new Signatures.Type(ElementType.I4)) {
                returnCode = this.Pop(ElementType.I4).I4;
            }
            else if (entryAssembly.EntryPoint.Signature.RetType.Type == new Signatures.Type(ElementType.U4)) {
                returnCode = this.Pop(ElementType.U4).U4;
            }
            else if (!entryAssembly.EntryPoint.Signature.RetType.IsVoid) {
                throw new ExecutionEngineException("Invalid entry point return type.");
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

                        case InstructionType.stloc_0: this.Pop(ref frame.Locals[0], frame.Method.LocalVariablesSignature.Locals[0].Type); break;
                        case InstructionType.stloc_1: this.Pop(ref frame.Locals[1], frame.Method.LocalVariablesSignature.Locals[1].Type); break;
                        case InstructionType.stloc_2: this.Pop(ref frame.Locals[2], frame.Method.LocalVariablesSignature.Locals[2].Type); break;
                        case InstructionType.stloc_3: this.Pop(ref frame.Locals[3], frame.Method.LocalVariablesSignature.Locals[3].Type); break;

                        case InstructionType.ldloc_0: this.Push(frame.Locals[0], frame.Method.LocalVariablesSignature.Locals[0].Type); break;
                        case InstructionType.ldloc_1: this.Push(frame.Locals[1], frame.Method.LocalVariablesSignature.Locals[1].Type); break;
                        case InstructionType.ldloc_2: this.Push(frame.Locals[2], frame.Method.LocalVariablesSignature.Locals[2].Type); break;
                        case InstructionType.ldloc_3: this.Push(frame.Locals[3], frame.Method.LocalVariablesSignature.Locals[3].Type); break;

                        case InstructionType.br_s: frame.InstructionPointer = inst.BranchInstruction; break;

                        //TODO Need to verify what is on the eval stack before and after a call
                        case InstructionType.ret: this.callStack.Pop(); goto end;
                        case InstructionType.call: this.callStack.Push(new CallFrame(frame.Method.Type.Assembly.FindMethod(inst.TableIndexOperand), 0)); goto end;

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
