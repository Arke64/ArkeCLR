using ArkeCLR.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Streams {
    public abstract class Heap<T> : Stream {
        private readonly Dictionary<int, T> cache = new Dictionary<int, T>();
        private readonly int offset;
        private readonly int scale;
        private ByteReader reader;

        public HeapType Type { get; }

        protected Heap(string name, HeapType type) : this(name, type, 0, 1) { }
        protected Heap(string name, HeapType type, int offset, int scale) : base(name) => (this.Type, this.offset, this.scale) = (type, offset, scale);

        protected abstract T Get(ByteReader reader);

        public override void Initialize(ByteReader reader) => this.reader = reader;

        public T GetAt(HeapToken token) => token.Heap == this.Type ? this.GetAt(token.Offset) : throw new ArgumentException("Invalid token type.", nameof(token));

        public T GetAt(uint index) => index <= int.MaxValue ? this.GetAt((int)index) : throw new ArgumentOutOfRangeException(nameof(index));

        public T GetAt(int index) {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));

            index = (index - this.offset) * this.scale;

            if (this.cache.TryGetValue(index, out var val))
                return val;

            this.reader.Seek(index, SeekOrigin.Begin);

            return this.cache[index] = this.Get(this.reader);
        }

        public IEnumerable<T> ReadAll() {
            while (this.reader.Position < this.reader.Length)
                yield return this.GetAt(this.reader.Position / this.scale + this.offset);
        }
    }

    public struct HeapToken {
        public HeapType Heap;
        public uint Offset;
    }

    public enum HeapType {
        String = 0,
        Guid = 1,
        Blob = 2,
        UserString = 3
    }
}
