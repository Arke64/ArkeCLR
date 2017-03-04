using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Streams {
    public class BlobStream : Stream<byte[]> {
        public override string Name => "#Blob";
        public override HeapType Type => HeapType.Blob;

        protected override byte[] Get() => this.reader.ReadArray<byte>(this.reader.ReadCompressedU4());

        public T GetAt<T>(HeapIndex index) where T : struct, ICustomByteReader {
            var obj = new T();

            obj.Read(new ByteReader(this.GetAt(index)));

            return obj;
        }
    }
}
