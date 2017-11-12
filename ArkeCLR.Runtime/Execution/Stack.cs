using ArkeCLR.Runtime.Logical;
using ArkeCLR.Runtime.Signatures;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Runtime.Execution {
    public class Stack {
        private readonly Stack<TypeRecord> evaluationStack = new Stack<TypeRecord>();
        private readonly Stack<CallFrame> callStack = new Stack<CallFrame>();

        public void EnsureEmpty() {
            if (this.evaluationStack.Any()) throw new ExecutionEngineException("Expected empty evaluation stack.");
            if (this.callStack.Any()) throw new ExecutionEngineException("Expected empty call stack.");
        }

        public CallFrame Current { get; private set; }

        public TypeRecord Pop() => this.evaluationStack.Pop();
        public TypeRecord Pop(Signatures.Type expected) => this.Pop(expected.ElementType);

        public void Pop(ref TypeRecord destination) => destination = this.Pop();
        public void Pop(ref TypeRecord destination, Signatures.Type expected) => destination = this.Pop(expected);

        public TypeRecord Pop(ElementType expected) {
            var result = this.Pop();

            if (result.Tag != expected)
                throw new ExecutionEngineException($"Popped {result.Tag} but expected {expected}.");

            return result;
        }

        public void Push(TypeRecord value) => this.evaluationStack.Push(value);
        public void Push(TypeRecord value, Signatures.Type expected) => this.Push(value, expected.ElementType);

        public void Push(TypeRecord value, ElementType expected) {
            if (value.Tag != expected)
                throw new ExecutionEngineException($"Pushed {value.Tag} but expected {expected}.");

            this.Push(value);
        }

        public void Call(Method method) => this.callStack.Push(this.Current = new CallFrame(method));

        public void Return() {
            var ret = default(TypeRecord);
            var frame = this.Current;

            if (!frame.Method.Signature.RetType.IsVoid)
                ret = this.Pop();

            for (var i = 0; i < frame.Method.Signature.ParamCount + (Interpreter.MethodHasThis(frame.Method.Signature) ? 1 : 0); i++)
                this.Pop();

            this.callStack.Pop();

            this.Current = this.callStack.Count > 0 ? this.callStack.Peek() : null;

            if (!frame.Method.Signature.RetType.IsVoid)
                this.Push(ret);
        }

        public class CallFrame {
            public uint InstructionPointer { get; set; }
            public Method Method { get; }
            public TypeRecord[] Args { get; }
            public TypeRecord[] Locals { get; }

            public CallFrame(Method method) {
                var hasThis = Interpreter.MethodHasThis(method.Signature);

                this.Method = method;
                this.InstructionPointer = 0;
                this.Args = new TypeRecord[method.Signature.ParamCount + (hasThis ? 1 : 0)];
                this.Locals = new TypeRecord[method.Locals.Count];

                if (hasThis)
                    this.Args[0].Tag = ElementType.Class; //TODO Need to record actual class, also valuetypes

                for (int i = 0, j = hasThis ? 1 : 0; i < method.Signature.ParamCount; i++, j++)
                    this.Args[j].Tag = method.Signature.Params[i].Type.ElementType;

                for (var i = 0; i < method.Locals.Count; i++)
                    this.Locals[i].Tag = method.Locals[i].Type.ElementType;
            }
        }
    }
}
