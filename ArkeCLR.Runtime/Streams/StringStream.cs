using ArkeCLR.Utilities;
using System.Text;

namespace ArkeCLR.Runtime.Streams {
    public class StringStream : Stream<string> {
        public StringStream(ByteReader reader) : base(reader, 0) { }

        protected override string Get() => this.reader.ReadStringTerminated(Encoding.UTF8);
    }
}
