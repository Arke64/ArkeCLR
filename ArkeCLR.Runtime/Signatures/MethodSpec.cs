using ArkeCLR.Utilities;
using System;

namespace ArkeCLR.Runtime.Signatures {
    public class MethodSpec : ICustomByteReader {
        public uint GenArgCount;
        public Type[] Types;

        public void Read(ByteReader reader) {
            var first = reader.ReadU1();

            if ((first & 0x0A) != 0) throw new InvalidOperationException();

            this.GenArgCount = reader.ReadCompressedU4();

            reader.ReadArray(this.GenArgCount, out this.Types);
        }
    }
}
