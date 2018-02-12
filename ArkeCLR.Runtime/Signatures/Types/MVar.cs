using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Signatures.Types {
    public class MVar : ICustomByteReader {
        public uint Number;

        public void Read(ByteReader reader) => reader.ReadCompressedU4();
    }
}
