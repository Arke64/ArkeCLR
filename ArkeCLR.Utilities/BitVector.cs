using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Utilities {
    public class BitVector : IEnumerable<bool> {
        private byte[] data;

        public int Count { get; }

        private bool Get(int index) => (this.data[index / 8] & (1 << (index % 8))) != 0;
        private void Set(int index) => this.data[index / 8] |= (byte)(1 << (index % 8));
        private void Clear(int index) => this.data[index / 8] &= (byte)~(1 << (index % 8));

        public BitVector(byte[] data, int bitCount) {
            if (data == null) throw new ArgumentNullException(nameof(bitCount));
            if (bitCount < 0) throw new ArgumentOutOfRangeException(nameof(bitCount));
            if (bitCount > data.Length * 8) throw new ArgumentException($"length cannot be greater than {data}.Length * 8.", nameof(bitCount));

            this.data = new byte[bitCount / 8 + ((bitCount % 8) > 0 ? 1 : 0)];
            this.Count = bitCount;

            Array.Copy(data, this.data, this.data.Length);
        }

        public BitVector(byte[] data) : this(data, (data?.Length * 8) ?? 0) { }
        public BitVector(byte data) : this(new[] { data }) { }
        public BitVector(ushort data) : this(BitConverter.GetBytes(data)) { }
        public BitVector(uint data) : this(BitConverter.GetBytes(data)) { }
        public BitVector(ulong data) : this(BitConverter.GetBytes(data)) { }
        public BitVector(sbyte data) : this(new [] { (byte)data }) { }
        public BitVector(short data) : this(BitConverter.GetBytes(data)) { }
        public BitVector(int data) : this(BitConverter.GetBytes(data)) { }
        public BitVector(long data) : this(BitConverter.GetBytes(data)) { }

        public bool this[int index] {
            get {
                if (index < 0 || index > this.Count) throw new ArgumentOutOfRangeException(nameof(index));

                return this.Get(index);
            }
            set {
                if (index < 0 || index > this.Count) throw new ArgumentOutOfRangeException(nameof(index));

                if (value) this.Set(index); else this.Clear(index);
            }
        }

        public int CountSet() => this.Count(i => i);

        public IEnumerator<bool> GetEnumerator() => new BitVectorEnumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private class BitVectorEnumerator : IEnumerator<bool> {
            private BitVector bitVector;
            private int index = -1;

            public BitVectorEnumerator(BitVector bitVector) => this.bitVector = bitVector;

            public bool Current => this.bitVector.Get(this.index);
            public bool MoveNext() => ++this.index < this.bitVector.Count;
            public void Reset() => this.index = 0;
            public void Dispose() => this.bitVector = null;
            object IEnumerator.Current => this.Current;
        }
    }
}
