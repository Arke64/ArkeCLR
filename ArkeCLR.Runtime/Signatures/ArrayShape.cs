using ArkeCLR.Utilities;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Signatures {
    public class ArrayShape : ICustomByteReader {
        public uint Rank;
        public uint NumSizes;
        public uint[] Size;
        public uint NumLoBounds;
        public uint[] LoBound;

        public void Read(ByteReader reader) {
            this.Rank = reader.ReadCompressedU4();

            this.NumSizes = reader.ReadCompressedU4();
            reader.ReadArray(this.NumSizes, out this.Size);

            this.NumLoBounds = reader.ReadCompressedU4();
            reader.ReadArray(this.NumLoBounds, out this.LoBound);
        }
    }
}
