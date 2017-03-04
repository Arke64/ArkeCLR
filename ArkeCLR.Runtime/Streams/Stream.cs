using ArkeCLR.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace ArkeCLR.Runtime.Streams {
    public abstract class Stream {
        public abstract string Name { get; }

        public abstract void Initialize(ByteReader reader);
    }

    public abstract class Stream<T> : Stream {
        private Dictionary<int, T> cache = new Dictionary<int, T>();

        protected ByteReader reader;

        protected virtual int Offset => 0;
        protected virtual int Scale => 1;

        public abstract HeapType Type { get; }

        public override void Initialize(ByteReader reader) => this.reader = reader;

        protected abstract T Get();

        public T GetAt(HeapIndex index) {
            if (index.Heap != this.Type) throw new ArgumentException("Invalid index type.", nameof(index));

            return this.GetAt(index.Offset);
        }

        public T GetAt(uint index) => this.GetAt((int)index);

        public T GetAt(int index) {
            index = (index - this.Offset) * this.Scale;

            if (index < 0)
                return default(T);

            if (this.cache.TryGetValue(index, out var val))
                return val;

            this.reader.Seek(index, SeekOrigin.Begin);

            return this.cache[index] = this.Get();
        }

        public IEnumerable<T> ReadAll() {
            while (this.reader.Position < this.reader.Length)
                yield return this.GetAt(this.reader.Position / this.Scale + this.Offset);
        }
    }
}
