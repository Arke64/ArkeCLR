using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Logical;
using ArkeCLR.Utilities;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Runtime {
    public class ExecutionEngine {
        private readonly IAssemblyResolver assemblyResolver;
        private readonly Action<string> logger;
        private readonly List<Assembly> assemblies = new List<Assembly>();
        private readonly Stack<ulong> evaluationStack = new Stack<ulong>();
        private readonly Stack<(Method, int)> methodStack = new Stack<(Method, int)>();

        public ExecutionEngine(IAssemblyResolver assemblyResolver, Action<string> logger) => (this.assemblyResolver, this.logger) = (assemblyResolver, logger);

        private Assembly Resolve(AssemblyName name) {
            var (found, data) = this.assemblyResolver.Resolve(name);

            if (!found)
                throw new CouldNotResolveAssemblyException(name);

            var assm = new Assembly(new CliFile(new ByteReader(data)));

            this.assemblies.Add(assm);

            return assm;
        }

        public int Run(string entryAssemblyPath) {
            var entryAssembly = this.Resolve(new AssemblyName(Path.GetFileNameWithoutExtension(entryAssemblyPath), entryAssemblyPath));

            this.methodStack.Push((entryAssembly.EntryPoint, 0));

            this.Run();

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

        private void Run() {
            do {
                var (method, ip) = this.methodStack.Pop();

                while (ip < method.Instructions.Count) {
                    var inst = method.Instructions[ip++];

                    switch (inst.Type) {
                        case InstructionType.nop: break;
                        case InstructionType.ldc_i4_2: this.PushI4(2); break;
                        case InstructionType.ret: ip = int.MaxValue; break;
                    }
                }
            } while (this.methodStack.Any());
        }
    }
}
