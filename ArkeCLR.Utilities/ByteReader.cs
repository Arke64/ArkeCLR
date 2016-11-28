using System;
using System.Runtime.InteropServices;

namespace ArkeCLR.Utilities {
    public struct ByteReader {
        private readonly byte[] buffer;
        private int position;

        public ByteReader(byte[] buffer) {
            if (!BitConverter.IsLittleEndian) throw new InvalidOperationException("Can only run on a Little Endian system.");

            this.buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            this.position = 0;
        }

        private T AdvanceAndReturn<T>(T result, int amount) {
            this.position += amount;

            return result;
        }

        private bool CanRead(int length, bool throwIfCant) {
            if (this.position + length > this.buffer.Length)
                return throwIfCant ? throw new IndexOutOfRangeException() : false;

            return true;
        }

        public void AdvanceTo(uint newPosition) => this.AdvanceTo((int)newPosition);

        public void AdvanceTo(int newPosition) {
            if (newPosition < 0 || newPosition > this.buffer.Length) throw new IndexOutOfRangeException();

            this.position = newPosition;
        }

        public byte ReadU8() => this.buffer[this.position++];
        public ushort ReadU16() => this.AdvanceAndReturn(BitConverter.ToUInt16(this.buffer, this.position), sizeof(ushort));
        public uint ReadU32() => this.AdvanceAndReturn(BitConverter.ToUInt32(this.buffer, this.position), sizeof(uint));
        public ulong ReadU64() => this.AdvanceAndReturn(BitConverter.ToUInt64(this.buffer, this.position), sizeof(ulong));
        public sbyte ReadI8() => (sbyte)this.buffer[this.position++];
        public short ReadI16() => this.AdvanceAndReturn(BitConverter.ToInt16(this.buffer, this.position), sizeof(short));
        public int ReadI32() => this.AdvanceAndReturn(BitConverter.ToInt32(this.buffer, this.position), sizeof(int));
        public long ReadI64() => this.AdvanceAndReturn(BitConverter.ToInt64(this.buffer, this.position), sizeof(long));

        public T ReadStruct<T>() where T : struct {
            var length = Marshal.SizeOf(default(T));
            var ptr = IntPtr.Zero;

            this.CanRead(length, true);

            try {
                ptr = Marshal.AllocHGlobal(length);

                Marshal.Copy(this.buffer, this.position, ptr, length);

                this.position += length;

                return Marshal.PtrToStructure<T>(ptr);
            }
            finally {
                if (ptr != IntPtr.Zero)
                    Marshal.FreeHGlobal(ptr);
            }
        }
    }
}
