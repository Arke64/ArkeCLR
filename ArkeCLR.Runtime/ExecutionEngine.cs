using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Logical;
using ArkeCLR.Utilities;
using System.IO;
using System;
using System.Collections.Generic;

namespace ArkeCLR.Runtime {
    public class ExecutionEngine {
        private readonly IAssemblyResolver assemblyResolver;
        private readonly Action<string> logger;
        private readonly List<Assembly> assemblies = new List<Assembly>();
        private readonly Stack<ulong> stack = new Stack<ulong>();

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

            entryAssembly.EntryPoint.Execute(this);

            if (entryAssembly.EntryPoint.Signature.RetType.IsVoid) {
                return 0;
            }
            else {
                return this.PopI4(); //TODO What are the valid entry point return types
            }
        }

        public byte PopU1() => (byte)this.stack.Pop();
        public ushort PopU2() => (ushort)this.stack.Pop();
        public uint PopU4() => (uint)this.stack.Pop();
        public ulong PopU8() => this.stack.Pop();
        public sbyte PopI1() => (sbyte)this.stack.Pop();
        public short PopI2() => (short)this.stack.Pop();
        public int PopI4() => (int)this.stack.Pop();
        public long PopI8() => (long)this.stack.Pop();

        public void PushU1(byte value) => this.stack.Push(value);
        public void PushU2(ushort value) => this.stack.Push(value);
        public void PushU3(uint value) => this.stack.Push(value);
        public void PushU4(ulong value) => this.stack.Push(value);
        public void PushI1(sbyte value) => this.stack.Push((ulong)value);
        public void PushI2(short value) => this.stack.Push((ulong)value);
        public void PushI3(int value) => this.stack.Push((ulong)value);
        public void PushI4(long value) => this.stack.Push((ulong)value);
    }
}
