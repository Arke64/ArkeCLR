using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Utilities {
    public class BitVector : IEnumerable<bool> {
        private readonly byte[] data;

        public int Count { get; }

        private bool Get(int index) => (this.data[index / 8] & (1 << (index % 8))) != 0;
        private void Set(int index) => this.data[index / 8] |= (byte)(1 << (index % 8));
        private void Clear(int index) => this.data[index / 8] &= (byte)~(1 << (index % 8));

        public BitVector(byte[] data) {
            if (data == null) throw new ArgumentNullException(nameof(data));

            this.Count = data.Length * 8;

            this.data = new byte[data.Length];

            Array.Copy(data, this.data, this.data.Length);
        }

        public BitVector(byte data) : this(new[] { data }) { }
        public BitVector(ushort data) : this(BitConverter.GetBytes(data)) { }
        public BitVector(uint data) : this(BitConverter.GetBytes(data)) { }
        public BitVector(ulong data) : this(BitConverter.GetBytes(data)) { }
        public BitVector(sbyte data) : this((byte)data) { }
        public BitVector(short data) : this(BitConverter.GetBytes(data)) { }
        public BitVector(int data) : this(BitConverter.GetBytes(data)) { }
        public BitVector(long data) : this(BitConverter.GetBytes(data)) { }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public IEnumerator<bool> GetEnumerator() {
            for (var i = 0; i < this.Count; i++)
                yield return this.Get(i);
        }

        public bool this[int index] {
            get {
                if (index < 0 || index >= this.Count) throw new ArgumentOutOfRangeException(nameof(index));

                return this.Get(index);
            }
            set {
                if (index < 0 || index >= this.Count) throw new ArgumentOutOfRangeException(nameof(index));

                if (value) this.Set(index); else this.Clear(index);
            }
        }

        public int CountSet() => this.Count(i => i);
    }
}
