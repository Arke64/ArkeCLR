using ArkeCLR.Runtime.Logical;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Runtime.Execution {
    public class CallFrame {
        public Method Method;
        public int InstructionPointer;

        public CallFrame(Method method, int instructionPointer) {
            this.Method = method;
            this.InstructionPointer = instructionPointer;
        }
    }

    public class Interpreter : IExecutionEngine {
        private readonly Stack<ulong> evaluationStack = new Stack<ulong>();
        private readonly Stack<CallFrame> callStack = new Stack<CallFrame>();

        public int Run(Assembly entryAssembly, IReadOnlyCollection<Assembly> references, Action<string> logger) {
            this.callStack.Push(new CallFrame(entryAssembly.EntryPoint, 0));

            do {
                var frame = this.callStack.Peek();

                while (frame.InstructionPointer < frame.Method.Instructions.Count) {
                    var inst = frame.Method.Instructions[frame.InstructionPointer++];

                    switch (inst.Type) {
                        case InstructionType.nop: break;
                        case InstructionType.ldc_i4_2: this.PushI4(2); break;
                        case InstructionType.ret: this.callStack.Pop(); goto end;
                        case InstructionType.call: this.callStack.Push(new CallFrame(frame.Method.Type.Assembly.FindMethod(inst.TableIndexOperand), 0)); goto end;
                        default: throw new NotImplementedException();
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
        private void PushU3(uint value) => this.evaluationStack.Push(value);
        private void PushU4(ulong value) => this.evaluationStack.Push(value);
        private void PushI1(sbyte value) => this.evaluationStack.Push((ulong)value);
        private void PushI2(short value) => this.evaluationStack.Push((ulong)value);
        private void PushI3(int value) => this.evaluationStack.Push((ulong)value);
        private void PushI4(long value) => this.evaluationStack.Push((ulong)value);
    }
}
