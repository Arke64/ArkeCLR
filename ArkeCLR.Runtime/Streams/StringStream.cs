using System.Text;

namespace ArkeCLR.Runtime.Streams {
    public class StringStream : Stream<string> {
        public override string Name => "#Strings";

        protected override string Get() => this.reader.ReadStringTerminated(Encoding.UTF8);
    }
}
