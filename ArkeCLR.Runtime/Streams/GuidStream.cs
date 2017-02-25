using System;

namespace ArkeCLR.Runtime.Streams {
    public class GuidStream : Stream<Guid> {
        public override string Name => "#GUID";
        protected override int Offset => 1;
        protected override int Scale => 16;

        protected override Guid Get() => new Guid(this.reader.ReadArray<byte>(16));
    }
}
