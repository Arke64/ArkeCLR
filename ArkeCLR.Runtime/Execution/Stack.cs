using ArkeCLR.Runtime.Logical;
using ArkeCLR.Runtime.Signatures;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Runtime.Execution {
    public class Stack {
        private readonly Stack<TypeRecord> evaluationStack = new Stack<TypeRecord>();
        private readonly Stack<CallFrame> callStack = new Stack<CallFrame>();

        public CallFrame Current { get; private set; }

        public void EnsureEmpty() {
            if (this.evaluationStack.Any()) throw new ExecutionEngineException("Expected empty evaluation stack.");
            if (this.callStack.Any()) throw new ExecutionEngineException("Expected empty call stack.");
        }

        //TODO Need to properly zero extend, sign extend, truncate, and round data pushed and popped to the stack following III.1.1, III.1.5, and III.1.6.

        public TypeRecord Pop() => this.evaluationStack.Pop();
        public TypeRecord Pop(Signatures.Type expected) => this.Pop(expected.ElementType); //TODO Need to verify the rest of the signature.

        public void Pop(ref TypeRecord destination) => destination = this.Pop();
        public void Pop(ref TypeRecord destination, Signatures.Type expected) => destination = this.Pop(expected);

        public TypeRecord Pop(ElementType expected) {
            var result = this.Pop();

            if (result.Tag != expected)
                throw new ExecutionEngineException($"Popped {result.Tag} but expected {expected}.");

            return result;
        }

        public void Push(TypeRecord value) => this.evaluationStack.Push(value);
        public void Push(TypeRecord value, Signatures.Type expected) => this.Push(value, expected.ElementType); //TODO Need to verify the rest of the signature.

        public void Push(TypeRecord value, ElementType expected) {
            if (value.Tag != expected)
                throw new ExecutionEngineException($"Pushed {value.Tag} but expected {expected}.");

            this.Push(value);
        }

        public void Call(Method method) {
            var frame = this.Current = new CallFrame(method);

            for (var i = method.ActualParameters - 1; i < uint.MaxValue; i--)
                frame.Arg(i).Assign(this.Pop());

            this.callStack.Push(frame);
        }

        public void Return() {
            this.callStack.Pop();

            this.Current = this.callStack.Count > 0 ? this.callStack.Peek() : null;
        }

        public class CallFrame {
            private readonly TypeRecord[] args;
            private readonly TypeRecord[] locals;

            public Method Method { get; }
            public uint InstructionPointer { get; set; }

            public ref TypeRecord Arg(uint index) => ref this.args[(int)index];
            public ref TypeRecord Local(uint index) => ref this.locals[(int)index];

            public CallFrame(Method method) {
                this.Method = method;
                this.InstructionPointer = 0;
                this.args = new TypeRecord[method.ActualParameters];
                this.locals = new TypeRecord[method.Locals.Count];

                if (!method.IsStatic)
                    this.args[0] = TypeRecord.FromType(method.Type);

                for (uint i = 0, j = method.FormatParametersStart; i < method.FormalParameters; i++, j++)
                    this.args[j] = TypeRecord.FromSignature(method.Signature.Params[i].Type);

                for (var i = 0; i < method.Locals.Count; i++)
                    this.locals[i] = TypeRecord.FromSignature(method.Locals[i].Type);
            }
        }
    }
}
