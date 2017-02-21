using ArkeCLR.Runtime.Headers;
using ArkeCLR.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ArkeCLR.Runtime.Streams {
    public abstract class Stream<T> {
        protected ByteReader reader;

        public Stream(ByteReader reader) => this.reader = reader;

        public T GetAt(int index) {
            this.reader.Seek(index, SeekOrigin.Begin);

            return this.Get();
        }

        protected abstract T Get();

        public IEnumerable<T> ReadAll() {
            while (this.reader.Position < this.reader.Length)
                yield return this.GetAt(this.reader.Position);
        }

        protected int ReadEncodedLength() {
            var first = this.reader.ReadU1();

            if ((first & 0b1000_0000) == 0b0000_0000) return first & 0b0111_1111;
            if ((first & 0b1100_0000) == 0b1000_0000) return ((first & 0b0011_1111) << 8) + this.reader.ReadU1();

            return ((first & 0b0001_1111) << 24) + (this.reader.ReadU1() << 16) + (this.reader.ReadU1() << 8) + this.reader.ReadU1();
        }
    }

    public class StringStream : Stream<string> {
        public StringStream(ByteReader reader) : base(reader) { }

        protected override string Get() => this.reader.ReadStringAligned(Encoding.UTF8);
    }

    public class BlobStream : Stream<byte[]> {
        public BlobStream(ByteReader reader) : base(reader) { }

        protected override byte[] Get() => this.reader.ReadArray<byte>(this.ReadEncodedLength());
    }

    public class UserStringStream : Stream<string> {
        public UserStringStream(ByteReader reader) : base(reader) { }

        protected override string Get() {
            var length = this.ReadEncodedLength();

            if (length == 0)
                return string.Empty;

            var data = this.reader.ReadArray<byte>(length);

            //TODO Do we need to do anything? See II.24.2.4
            if (data[data.Length - 1] == 1) { }

            return Encoding.Unicode.GetString(data, 0, data.Length - 1);
        }
    }

    public class GuidStream : Stream<Guid> {
        public GuidStream(ByteReader reader) : base(reader) { }

        protected override Guid Get() => new Guid(this.reader.ReadArray<byte>(16));
    }

    //TODO Keep heap and simple index sizes in mind. See II.24.2.6
    public class TableStream {
        private ByteReader reader;

        public CilTableStreamHeader Header { get; }

        public TableStream(ByteReader reader) {
            this.reader = reader;

            this.Header = reader.ReadCustom<CilTableStreamHeader>();
        }
    }
}
