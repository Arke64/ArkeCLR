﻿using ArkeCLR.Utilities;
using System.Collections.Generic;
using System.IO;

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

            this.reader.Seek(index - this.offset, SeekOrigin.Begin);

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
}
