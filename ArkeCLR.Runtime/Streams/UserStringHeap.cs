using ArkeCLR.Utilities;
using System.Text;

namespace ArkeCLR.Runtime.Streams {
    public class UserStringHeap : Heap<string> {
        public UserStringHeap() : base("#US", HeapType.UserString) { }

        protected override string Get(ByteReader reader) {
            var length = reader.ReadCompressedU4();

            if (length == 0)
                return string.Empty;

            var data = reader.ReadArray<byte>(length);

            //TODO Do we need to do anything here? See II.24.2.4
            //if (data[data.Length - 1] == 1)

            return Encoding.Unicode.GetString(data, 0, data.Length - 1);
        }
    }
}
