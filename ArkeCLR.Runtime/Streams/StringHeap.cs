using ArkeCLR.Utilities;
using System.Text;

namespace ArkeCLR.Runtime.Streams {
    public class StringHeap : Heap<string> {
        public StringHeap() : base("#Strings", HeapType.String) { }

        protected override string Get(ByteReader reader) => reader.ReadStringTerminated(Encoding.UTF8);
    }
}
