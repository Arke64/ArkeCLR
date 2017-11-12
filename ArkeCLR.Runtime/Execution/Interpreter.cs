using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Logical;
using ArkeCLR.Runtime.Signatures;
using System;
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

    public class Interpreter : IExecutionEngine {
        private readonly Action<string> logger;
        private readonly TypeSystem typeSystem;
        private readonly Stack stack;

        public static bool MethodHasThis(MethodDefSig sig) => (sig.Flags & SignatureFlags.HasThis) != 0;
        public static bool MethodHasExplicitThis(MethodDefSig sig) => (sig.Flags & SignatureFlags.ExplicitThis) != 0;

        public Interpreter(IFileResolver fileResolver, Action<string> logger) => (this.typeSystem, this.stack, this.logger) = (new TypeSystem(fileResolver, logger), new Stack(), logger);

        public long Run(string entryAssemblyPath) {
            var entryPoint = this.typeSystem.Load(entryAssemblyPath).EntryPoint;

            if (Interpreter.MethodHasThis(entryPoint.Signature) || Interpreter.MethodHasExplicitThis(entryPoint.Signature)) throw new InvalidAssemblyException("Entry point must be static.");
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
