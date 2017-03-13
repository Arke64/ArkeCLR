using ArkeCLR.Runtime.Logical;
using ArkeCLR.Runtime.Signatures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Runtime.Execution {
    public class CallFrame {
        public Method Method;
        public int InstructionPointer;
        public ulong[] Locals; //TODO Need to track types and store more than numerics

        public CallFrame(Method method, int instructionPointer) {
            this.Method = method;
            this.InstructionPointer = instructionPointer;
            this.Locals = new ulong[method.LocalVariablesSignature.Count];
        }
    }

    public class Interpreter : IExecutionEngine {
        private readonly Stack<ulong> evaluationStack = new Stack<ulong>(); //TODO need to track types and store more than numerics
        private readonly Stack<CallFrame> callStack = new Stack<CallFrame>();

        public int Run(Assembly entryAssembly, IReadOnlyCollection<Assembly> references, Action<string> logger) {
            this.callStack.Push(new CallFrame(entryAssembly.EntryPoint, 0));

            do {
                var frame = this.callStack.Peek();

                while (frame.InstructionPointer < frame.Method.Instructions.Count) {
                    var inst = frame.Method.Instructions[frame.InstructionPointer++];

                    switch (inst.Type) {
                        default: throw new NotImplementedException();

                        case InstructionType.nop: break;

                        case InstructionType.ldc_i4_0: this.PushI4(0); break;
                        case InstructionType.ldc_i4_1: this.PushI4(1); break;
                        case InstructionType.ldc_i4_2: this.PushI4(2); break;
                        case InstructionType.ldc_i4_3: this.PushI4(3); break;
                        case InstructionType.ldc_i4_4: this.PushI4(4); break;
                        case InstructionType.ldc_i4_5: this.PushI4(5); break;
                        case InstructionType.ldc_i4_6: this.PushI4(6); break;
                        case InstructionType.ldc_i4_7: this.PushI4(7); break;
                        case InstructionType.ldc_i4_8: this.PushI4(8); break;

                        case InstructionType.stloc_0: frame.Locals[0] = this.Pop(frame.Method.LocalVariablesSignature.Locals[0].Type.ElementType); break;
                        case InstructionType.stloc_1: frame.Locals[1] = this.Pop(frame.Method.LocalVariablesSignature.Locals[1].Type.ElementType); break;
                        case InstructionType.stloc_2: frame.Locals[2] = this.Pop(frame.Method.LocalVariablesSignature.Locals[2].Type.ElementType); break;
                        case InstructionType.stloc_3: frame.Locals[3] = this.Pop(frame.Method.LocalVariablesSignature.Locals[3].Type.ElementType); break;

                        case InstructionType.ldloc_0: this.Push(frame.Locals[0], frame.Method.LocalVariablesSignature.Locals[0].Type.ElementType); break;
                        case InstructionType.ldloc_1: this.Push(frame.Locals[1], frame.Method.LocalVariablesSignature.Locals[1].Type.ElementType); break;
                        case InstructionType.ldloc_2: this.Push(frame.Locals[2], frame.Method.LocalVariablesSignature.Locals[2].Type.ElementType); break;
                        case InstructionType.ldloc_3: this.Push(frame.Locals[3], frame.Method.LocalVariablesSignature.Locals[3].Type.ElementType); break;

                        case InstructionType.br_s: frame.InstructionPointer = inst.BranchInstruction; break;

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

            if (entryAssembly.EntryPoint.Signature.RetType.IsVoid) {
                return 0;
            }
            else {
                return this.PopI4(); //TODO What are the valid entry point return types
            }
        }

        private ulong Pop(ElementType type) {
            switch (type) {
                case ElementType.I1: return (ulong)this.PopI1();
                case ElementType.I2: return (ulong)this.PopI2();
                case ElementType.I4: return (ulong)this.PopI4();
                case ElementType.I8: return (ulong)this.PopI8();
                case ElementType.U1: return this.PopU1();
                case ElementType.U2: return this.PopU2();
                case ElementType.U4: return this.PopU4();
                case ElementType.U8: return this.PopU8();
                default: throw new NotImplementedException();
            }
        }

        private void Push(ulong value, ElementType type) {
            switch (type) {
                case ElementType.I1: this.PushI1((sbyte)value); break;
                case ElementType.I2: this.PushI2((short)value); break;
                case ElementType.I4: this.PushI4((int)value); break;
                case ElementType.I8: this.PushI8((long)value); break;
                case ElementType.U1: this.PushU1((byte)value); break;
                case ElementType.U2: this.PushU2((ushort)value); break;
                case ElementType.U4: this.PushU4((uint)value); break;
                case ElementType.U8: this.PushU8((ulong)value); break;
                default: throw new NotImplementedException();
            }
        }

        private byte PopU1() => (byte)this.evaluationStack.Pop();
        private ushort PopU2() => (ushort)this.evaluationStack.Pop();
        private uint PopU4() => (uint)this.evaluationStack.Pop();
        private ulong PopU8() => this.evaluationStack.Pop();
        private sbyte PopI1() => (sbyte)this.evaluationStack.Pop();
        private short PopI2() => (short)this.evaluationStack.Pop();
        private int PopI4() => (int)this.evaluationStack.Pop();
        private long PopI8() => (long)this.evaluationStack.Pop();

        private void PushU1(byte value) => this.evaluationStack.Push(value);
        private void PushU2(ushort value) => this.evaluationStack.Push(value);
        private void PushU4(uint value) => this.evaluationStack.Push(value);
        private void PushU8(ulong value) => this.evaluationStack.Push(value);
        private void PushI1(sbyte value) => this.evaluationStack.Push((ulong)value);
        private void PushI2(short value) => this.evaluationStack.Push((ulong)value);
        private void PushI4(int value) => this.evaluationStack.Push((ulong)value);
        private void PushI8(long value) => this.evaluationStack.Push((ulong)value);
    }
}
