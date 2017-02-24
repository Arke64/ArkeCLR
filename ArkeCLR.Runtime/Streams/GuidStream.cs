using ArkeCLR.Utilities;
using System;

namespace ArkeCLR.Runtime.Streams {
    public class GuidStream : Stream<Guid?> {
        public GuidStream(ByteReader reader) : base(reader, 1) { }

        protected override Guid? Get() => new Guid(this.reader.ReadArray<byte>(16));
    }
}
