namespace ArkeCLR.Runtime.Streams {
    public class BlobStream : Stream<byte[]> {
        public override string Name => "#Blob";

        protected override byte[] Get() => this.reader.ReadArray<byte>(this.reader.ReadCompressedU4());
    }
}
