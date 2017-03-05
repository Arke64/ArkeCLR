using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Logical {
    public struct Instruction : ICustomByteReader {
        public InstructionType Type;
        public ExtendedInstructionType ExtendedType;

        public void Read(ByteReader body) {
            body.ReadEnum(ref this.Type);

            if (this.Type == InstructionType.extended)
                body.ReadEnum(ref this.ExtendedType);
        }

        public void Execute(ExecutionEngine engine, ref int ip) {
            switch (this.Type) {
                case InstructionType.nop: break;
                case InstructionType.ldc_i4_2: engine.PushI4(2); break;
                case InstructionType.ret: ip = int.MaxValue; break;
            }
        }
    }
}
