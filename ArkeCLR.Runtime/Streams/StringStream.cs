using System.Text;

namespace ArkeCLR.Runtime.Streams {
    public class StringStream : Stream<string> {
        public override string Name => "#Strings";
        public override HeapType Type => HeapType.String;

        protected override string Get() => this.reader.ReadStringTerminated(Encoding.UTF8);
    }
}
