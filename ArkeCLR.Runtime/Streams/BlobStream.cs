using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Streams {
    public class BlobHeap : Heap<byte[]> {
        public BlobHeap() : base("#Blob", HeapType.Blob) { }

        protected override byte[] Get(ByteReader reader) => reader.ReadArray<byte>(reader.ReadCompressedU4());

        public void GetAt<T>(HeapIndex index, out T value) where T : struct, ICustomByteReader => value = this.GetAt<T>(index);

        public T GetAt<T>(HeapIndex index) where T : struct, ICustomByteReader {
            var obj = new T();

            obj.Read(new ByteReader(this.GetAt(index)));

            return obj;
        }
    }
}
