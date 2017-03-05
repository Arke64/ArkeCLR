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
    }
}
