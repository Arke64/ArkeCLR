using System;

namespace ArkeCLR.Runtime.Streams {
    //TODO Need to scale index by 16 and offset it by 1
    public class GuidStream : Stream<Guid> {
        public override string Name => "#GUID";

        protected override Guid Get() => new Guid(this.reader.ReadArray<byte>(16));
    }
}
