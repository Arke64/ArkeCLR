using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Streams {
    public class BlobStream : Stream<byte[]> {
        public BlobStream(ByteReader reader) : base(reader, 0) { }

        protected override byte[] Get() => this.reader.ReadArray<byte>(this.ReadEncodedLength());
    }
}
