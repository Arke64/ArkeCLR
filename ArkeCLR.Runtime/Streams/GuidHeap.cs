using ArkeCLR.Utilities;
using System;

namespace ArkeCLR.Runtime.Streams {
    public class GuidHeap : Heap<Guid> {
        public GuidHeap() : base("#GUID", HeapType.Guid, 1, 16) { }

        protected override Guid Get(ByteReader reader) => new Guid(reader.ReadArray<byte>(16));
    }
}
