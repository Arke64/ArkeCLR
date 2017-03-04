using System.Text;

namespace ArkeCLR.Runtime.Streams {
    public class UserStringStream : Stream<string> {
        public override string Name => "#US";
        public override HeapType Type => HeapType.UserString;

        protected override string Get() {
            var length = this.reader.ReadCompressedU4();

            if (length == 0)
                return string.Empty;

            var data = this.reader.ReadArray<byte>(length);

            //TODO Do we need to do anything? See II.24.2.4
            if (data[data.Length - 1] == 1) { }

            return Encoding.Unicode.GetString(data, 0, data.Length - 1);
        }
    }
}
