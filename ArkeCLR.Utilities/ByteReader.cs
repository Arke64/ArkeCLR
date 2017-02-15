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
        private int position;

        public ByteReader(byte[] buffer) {
            if (!BitConverter.IsLittleEndian) throw new InvalidOperationException("Can only run on a little endian system.");

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

        public void Seek(uint position, SeekOrigin seekOrigin) => this.Seek((int)position, seekOrigin);

        public void Seek(int position, SeekOrigin seekOrigin) {
            if (seekOrigin == SeekOrigin.Begin) {
                if (position < 0 || position > this.buffer.Length) throw new IndexOutOfRangeException();

                this.position = position;
            }
            else if (seekOrigin == SeekOrigin.Current) {
                var newPosition = this.position + position;

                if (newPosition < 0 || newPosition > this.buffer.Length) throw new IndexOutOfRangeException();

                this.position = newPosition;
            }
            else {
                throw new NotSupportedException();
            }
        }

        //TODO These need to check for reading beyond the end
        public byte ReadU8() => this.buffer[this.position++];
        public ushort ReadU16() => this.AdvanceAndReturn(BitConverter.ToUInt16(this.buffer, this.position), sizeof(ushort));
        public uint ReadU32() => this.AdvanceAndReturn(BitConverter.ToUInt32(this.buffer, this.position), sizeof(uint));
        public ulong ReadU64() => this.AdvanceAndReturn(BitConverter.ToUInt64(this.buffer, this.position), sizeof(ulong));
        public sbyte ReadI8() => (sbyte)this.buffer[this.position++];
        public short ReadI16() => this.AdvanceAndReturn(BitConverter.ToInt16(this.buffer, this.position), sizeof(short));
        public int ReadI32() => this.AdvanceAndReturn(BitConverter.ToInt32(this.buffer, this.position), sizeof(int));
        public long ReadI64() => this.AdvanceAndReturn(BitConverter.ToInt64(this.buffer, this.position), sizeof(long));

        public string ReadStringFixed(Encoding encoding, uint length) => this.ReadStringFixed(encoding, (int)length);
        public string ReadStringFixed(Encoding encoding, uint length, byte padder) => this.ReadStringFixed(encoding, (int)length, padder);

        public string ReadStringFixed(Encoding encoding, int length) => encoding.GetString(this.ReadBytes(length));
        public string ReadStringFixed(Encoding encoding, int length, byte padder) => encoding.GetString(this.ReadBytes(length).TakeWhile(b => b != padder).ToArray());

        public string ReadStringAligned(Encoding encoding) => this.ReadStringAligned(encoding, 0, 1);
        public string ReadStringAligned(Encoding encoding, byte padder) => this.ReadStringAligned(encoding, padder, 1);

        public string ReadStringAligned(Encoding encoding, byte padder, int paddingMultiple) {
            var bytes = new List<byte>();

            while (true) {
                var cur = this.ReadU8();

                if (cur == padder)
                    break;

                bytes.Add(cur);
            }

            var diff = MathEx.RoundUpToNearestMultiple(this.position, paddingMultiple) - this.position;

            this.CanRead(diff, true);

            this.position += diff;

            return encoding.GetString(bytes.ToArray());
        }

        public byte[] ReadBytes(uint length) => this.ReadBytes((int)length);

        public byte[] ReadBytes(int length) {
            if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));

            this.CanRead(length, true);

            var buffer = new byte[length];

            Array.Copy(this.buffer, this.position, buffer, 0, length);

            this.position += length;

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
