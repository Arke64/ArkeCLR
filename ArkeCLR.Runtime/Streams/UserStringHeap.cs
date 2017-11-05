using ArkeCLR.Utilities;
using System;
using System.Text;

namespace ArkeCLR.Runtime.Streams {
    public class UserStringHeap : Heap<string> {
        public UserStringHeap() : base("#US", HeapType.UserString) { }

        protected override string Get(ByteReader reader) {
            var length = reader.ReadCompressedU4();

            if (length == 0)
                return string.Empty;

            var data = reader.ReadArray<byte>(length);

            if (data[data.Length - 1] == 1)
                throw new NotImplementedException("See II.24.2.4");

            return Encoding.Unicode.GetString(data, 0, data.Length - 1);
        }
    }
}
