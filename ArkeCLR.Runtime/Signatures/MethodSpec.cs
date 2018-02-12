using ArkeCLR.Utilities;
using System;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Signatures {
    public class MethodSpec : ICustomByteReader {
        public uint GenArgCount;
        public Type[] Types;

        public void Read(ByteReader reader) {
            if (!reader.TryReadEnum(SignatureFlags.GenericInst)) throw new InvalidOperationException();

            this.GenArgCount = reader.ReadCompressedU4();

            reader.ReadArray(this.GenArgCount, out this.Types);
        }
    }
}
