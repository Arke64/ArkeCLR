using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Streams {
    public class BlobHeap : Heap<byte[]> {
        public BlobHeap() : base("#Blob", HeapType.Blob) { }

        protected override byte[] Get(ByteReader reader) => reader.ReadArray<byte>(reader.ReadCompressedU4());

        public void GetAt<T>(HeapToken token, out T value) where T : ICustomByteReader, new() => value = this.GetAt<T>(token);

        public T GetAt<T>(HeapToken token) where T : ICustomByteReader, new() {
            var obj = new T();

            obj.Read(new ByteReader(this.GetAt(token)));

            return obj;
        }
    }
}
