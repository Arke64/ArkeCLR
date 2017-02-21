using ArkeCLR.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ArkeCLR.Utilities {
    public class ByteReader {
        private readonly byte[] buffer;

        public int Position { get; private set; }
        public int Length => this.buffer.Length;

        public ByteReader(byte[] buffer) {
            if (!BitConverter.IsLittleEndian) throw new InvalidOperationException("Can only run on a little endian system.");

            this.buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));

            this.Position = 0;
        }

        private T AdvanceAndReturn<T>(T result, int amount) {
            this.Position += amount;

            return result;
        }

        private bool CanRead(int length, bool throwIfCant) => (length == 0 || (this.Position + length <= this.Length && length > 0)) || (throwIfCant ? throw new IndexOutOfRangeException() : false);

        public void Seek(uint position, SeekOrigin seekOrigin) => this.Seek((int)position, seekOrigin);

        public void Seek(int position, SeekOrigin seekOrigin) {
            if (seekOrigin == SeekOrigin.Begin) {
                if (position < 0 || position > this.Length) throw new IndexOutOfRangeException();

                this.Position = position;
            }
            else if (seekOrigin == SeekOrigin.Current) {
                var newPosition = this.Position + position;

                if (newPosition < 0 || newPosition > this.Length) throw new IndexOutOfRangeException();

                this.Position = newPosition;
            }
            else {
                throw new NotSupportedException();
            }
        }

        public ByteReader CreateView(uint offset, uint size) {
            if (offset + size > this.Length) throw new IndexOutOfRangeException();

            var buffer = new byte[size];

            Array.Copy(this.buffer, (int)offset, buffer, 0, (int)size);

            return new ByteReader(buffer);
        }

        //TODO These need to check for reading beyond the end
        public byte ReadU1() => this.buffer[this.Position++];
        public ushort ReadU2() => this.AdvanceAndReturn(BitConverter.ToUInt16(this.buffer, this.Position), sizeof(ushort));
        public uint ReadU4() => this.AdvanceAndReturn(BitConverter.ToUInt32(this.buffer, this.Position), sizeof(uint));
        public ulong ReadU8() => this.AdvanceAndReturn(BitConverter.ToUInt64(this.buffer, this.Position), sizeof(ulong));
        public sbyte ReadI1() => (sbyte)this.buffer[this.Position++];
        public short ReadI2() => this.AdvanceAndReturn(BitConverter.ToInt16(this.buffer, this.Position), sizeof(short));
        public int ReadI4() => this.AdvanceAndReturn(BitConverter.ToInt32(this.buffer, this.Position), sizeof(int));
        public long ReadI8() => this.AdvanceAndReturn(BitConverter.ToInt64(this.buffer, this.Position), sizeof(long));

        public string ReadStringFixed(Encoding encoding, uint length) => this.ReadStringFixed(encoding, (int)length);
        public string ReadStringFixed(Encoding encoding, uint length, byte padder) => this.ReadStringFixed(encoding, (int)length, padder);

        public string ReadStringFixed(Encoding encoding, int length) => encoding.GetString(this.ReadArray<byte>(length));
        public string ReadStringFixed(Encoding encoding, int length, byte padder) => encoding.GetString(this.ReadArray<byte>(length).TakeWhile(b => b != padder).ToArray());

        public string ReadStringAligned(Encoding encoding) => this.ReadStringAligned(encoding, 0, 1);
        public string ReadStringAligned(Encoding encoding, byte padder) => this.ReadStringAligned(encoding, padder, 1);

        public string ReadStringAligned(Encoding encoding, byte padder, int paddingMultiple) {
            var bytes = new List<byte>();

            while (true) {
                var cur = this.ReadU1();

                if (cur == padder)
                    break;

                bytes.Add(cur);
            }

            var diff = MathEx.RoundUpToNearestMultiple(this.Position, paddingMultiple) - this.Position;

            this.CanRead(diff, true);

            this.Position += diff;

            return encoding.GetString(bytes.ToArray());
        }

        public T[] ReadArray<T>(uint count) => this.ReadArray<T>((int)count);

        public T[] ReadArray<T>(int count) {
            var size = Marshal.SizeOf(default(T));
            var bytes = count * size;

            this.CanRead(bytes, true);

            var buffer = new T[count];

            Buffer.BlockCopy(this.buffer, this.Position, buffer, 0, bytes);

            this.Position += bytes;

            return buffer;
        }

        public T ReadCustom<T>() where T : struct, ICustomByteReader {
            var result = new T();

            result.Read(this);

            return result;
        }

        public T[] ReadCustom<T>(uint count) where T : struct, ICustomByteReader => this.ReadStruct<T>((int)count);

        public T[] ReadCustom<T>(int count) where T : struct, ICustomByteReader {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            var result = new T[count];

            for (var i = 0; i < count; i++)
                result[i] = this.ReadCustom<T>();

            return result;
        }

        public T ReadStruct<T>() where T : struct {
            var size = Marshal.SizeOf(default(T));
            var ptr = IntPtr.Zero;

            this.CanRead(size, true);

            try {
                ptr = Marshal.AllocHGlobal(size);

                Marshal.Copy(this.buffer, this.Position, ptr, size);

                this.Position += size;

                return Marshal.PtrToStructure<T>(ptr);
            }
            finally {
                if (ptr != IntPtr.Zero)
                    Marshal.FreeHGlobal(ptr);
            }
        }

        public T[] ReadStruct<T>(uint count) where T : struct => this.ReadStruct<T>((int)count);

        public T[] ReadStruct<T>(int count) where T : struct {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            var result = new T[count];

            for (var i = 0; i < count; i++)
                result[i] = this.ReadStruct<T>();

            return result;
        }
    }

    public interface ICustomByteReader {
        void Read(ByteReader reader);
    }
}
