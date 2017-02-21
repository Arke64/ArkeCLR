using ArkeCLR.Runtime.FileFormats;
using ArkeCLR.Runtime.Headers;
using ArkeCLR.Runtime.Tables;
using ArkeCLR.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ArkeCLR.Runtime.Streams {
    public abstract class Stream<T> {
        private Dictionary<int, T> cache = new Dictionary<int, T>();

        protected readonly ByteReader reader;
        private readonly int offset;

        public Stream(ByteReader reader, int offset) => (this.reader, this.offset) = (reader, offset);

        public T GetAt(uint index) => this.GetAt((int)index);

        public T GetAt(int index) {
            if (this.cache.TryGetValue(index, out var val))
                return val;

            var actual = index - this.offset;

            if (actual < 0)
                return default(T);

            this.reader.Seek(actual, SeekOrigin.Begin);

            return this.cache[index] = this.Get();
        }

        protected abstract T Get();

        public IEnumerable<T> ReadAll() {
            while (this.reader.Position < this.reader.Length)
                yield return this.GetAt(this.reader.Position + this.offset);
        }

        protected int ReadEncodedLength() {
            var first = this.reader.ReadU1();

            if ((first & 0b1000_0000) == 0b0000_0000) return first & 0b0111_1111;
            if ((first & 0b1100_0000) == 0b1000_0000) return ((first & 0b0011_1111) << 8) + this.reader.ReadU1();

            return ((first & 0b0001_1111) << 24) + (this.reader.ReadU1() << 16) + (this.reader.ReadU1() << 8) + this.reader.ReadU1();
        }
    }

    public class StringStream : Stream<string> {
        public StringStream(ByteReader reader) : base(reader, 0) { }

        protected override string Get() => this.reader.ReadStringAligned(Encoding.UTF8);
    }

    public class BlobStream : Stream<byte[]> {
        public BlobStream(ByteReader reader) : base(reader, 0) { }

        protected override byte[] Get() => this.reader.ReadArray<byte>(this.ReadEncodedLength());
    }

    public class UserStringStream : Stream<string> {
        public UserStringStream(ByteReader reader) : base(reader, 0) { }

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

    public class GuidStream : Stream<Guid?> {
        public GuidStream(ByteReader reader) : base(reader, 1) { }

        protected override Guid? Get() => new Guid(this.reader.ReadArray<byte>(16));
    }

    //TODO Keep heap and simple index sizes in mind. See II.24.2.6
    public class TableStream {
        private readonly ByteReader reader;
        private readonly CliFile parent;

        public CilTableStreamHeader Header { get; }
        public IReadOnlyList<Module> Modules { get; private set; }

        public TableStream(CliFile parent, ByteReader reader) {
            this.reader = reader;
            this.parent = parent;

            this.Header = reader.ReadCustom<CilTableStreamHeader>();
        }

        public void ParseTables() {
            IReadOnlyList<T> read<T>(TableType table) where T : struct, ICustomByteReader<CliFile> => this.reader.ReadCustom<T, CliFile>(this.Header.Rows[(int)table], this.parent);

            this.Modules = read<Module>(TableType.Module);
        }

        public uint ReadHeapIndex(HeapType heap) => this.Header.HeapSizes[(int)heap] ? this.reader.ReadU4() : this.reader.ReadU2();
    }

    public enum HeapType {
        String = 0,
        Guid = 1,
        Blob = 2
    } 

    public enum TableType {
        Module = 0
    }
}
