using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Signatures.Types {
    public class Var : ICustomByteReader {
        public uint Number;

        public void Read(ByteReader reader) => reader.ReadCompressedU4();
    }
}
