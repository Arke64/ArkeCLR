using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ArkeCLR.Utilities {
    public class ByteReader {
        private readonly byte[] buffer;

        public int Position { get; private set; }
        public int Length { get; }

        protected ByteReader(ByteReader reader) : this(reader.buffer) { }

        public ByteReader(byte[] buffer) : this(buffer, 0, buffer?.Length ?? 0) { }

        public ByteReader(byte[] buffer, int offset, int size) {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (size <= 0 || checked(offset + size) > buffer.Length) throw new ArgumentOutOfRangeException(nameof(size));

            this.buffer = new byte[size];

            Array.Copy(buffer, offset, this.buffer, 0, size);

            this.Length = size;
            this.Position = 0;
        }

        public void Seek(uint position, SeekOrigin seekOrigin) {
            if (position > int.MaxValue) throw new ArgumentOutOfRangeException(nameof(position));

            this.Seek((int)position, seekOrigin);
        }

        public void Seek(int position, SeekOrigin seekOrigin) {
            switch (seekOrigin) {
                case SeekOrigin.Begin: this.Position = position; break;
                case SeekOrigin.Current: this.Position += position; break;
                case SeekOrigin.End: this.Position = this.Length + position; break;
            }
        }

        public void SeekToMultiple(int multiple) => this.Position += multiple - this.Position % multiple;

        public ByteReader CreateView(int offset, int size) => new ByteReader(this.buffer, offset, size);

        public ByteReader CreateView(uint offset, uint size) {
            if (offset > int.MaxValue) throw new ArgumentOutOfRangeException(nameof(offset));
            if (size > int.MaxValue) throw new ArgumentOutOfRangeException(nameof(size));

            return this.CreateView((int)offset, (int)size);
        }

        public byte PeekU1() => this.buffer[this.Position];
        public ushort PeekU2() => BitConverter.ToUInt16(this.buffer, this.Position);
        public uint PeekU4() => BitConverter.ToUInt32(this.buffer, this.Position);
        public ulong PeekU8() => BitConverter.ToUInt64(this.buffer, this.Position);
        public sbyte PeekI1() => (sbyte)this.buffer[this.Position];
        public short PeekI2() => BitConverter.ToInt16(this.buffer, this.Position);
        public int PeekI4() => BitConverter.ToInt32(this.buffer, this.Position);
        public long PeekI8() => BitConverter.ToInt64(this.buffer, this.Position);

        public T PeekEnum<T>() {
            var type = Enum.GetUnderlyingType(typeof(T));

            if (type == typeof(byte)) return (T)(object)this.PeekU1();
            else if (type == typeof(ushort)) return (T)(object)this.PeekU2();
            else if (type == typeof(uint)) return (T)(object)this.PeekU4();
            else if (type == typeof(ulong)) return (T)(object)this.PeekU8();
            else if (type == typeof(sbyte)) return (T)(object)this.PeekI1();
            else if (type == typeof(short)) return (T)(object)this.PeekI2();
            else if (type == typeof(int)) return (T)(object)this.PeekI4();
            else if (type == typeof(long)) return (T)(object)this.PeekI8();
            else throw new NotSupportedException();
        }

        public bool TryPeekEnum<T>(T expected) => this.PeekEnum<T>().Equals(expected);

        public bool TryPeekEnum<T>(out T result, params T[] expected) {
            var a = this.PeekEnum<T>();

            foreach (var e in expected) {
                if (e.Equals(a)) {
                    result = a;

                    return true;
                }
            }

            result = default;

            return false;
        }

        public bool TryReadEnum<T>(T expected) {
            var res = this.PeekEnum<T>().Equals(expected);

            if (res)
                this.ReadEnum<T>();

            return res;
        }

        public bool TryReadEnum<T>(out T result, params T[] expected) {
            var a = this.PeekEnum<T>();

            foreach (var e in expected) {
                if (e.Equals(a)) {
                    result = a;

                    this.ReadEnum<T>();

                    return true;
                }
            }

            result = default;

            return false;
        }

        public byte ReadU1() => this.buffer[this.Position++];
        public ushort ReadU2() { var r = BitConverter.ToUInt16(this.buffer, this.Position); this.Position += sizeof(ushort); return r; }
        public uint ReadU4() { var r = BitConverter.ToUInt32(this.buffer, this.Position); this.Position += sizeof(uint); return r; }
        public ulong ReadU8() { var r = BitConverter.ToUInt64(this.buffer, this.Position); this.Position += sizeof(ulong); return r; }
        public sbyte ReadI1() => (sbyte)this.buffer[this.Position++];
        public short ReadI2() { var r = BitConverter.ToInt16(this.buffer, this.Position); this.Position += sizeof(short); return r; }
        public int ReadI4() { var r = BitConverter.ToInt32(this.buffer, this.Position); this.Position += sizeof(int); return r; }
        public long ReadI8() { var r = BitConverter.ToInt64(this.buffer, this.Position); this.Position += sizeof(long); return r; }
        public float ReadR4() { var r = BitConverter.ToSingle(this.buffer, this.Position); this.Position += sizeof(float); return r; }
        public double ReadR8() { var r = BitConverter.ToDouble(this.buffer, this.Position); this.Position += sizeof(double); return r; }

        public uint ReadCompressedU4() {
            var first = this.ReadU1();

            if ((first & 0b1000_0000) == 0b0000_0000) return first & 0b0111_1111U;
            if ((first & 0b1100_0000) == 0b1000_0000) return ((first & 0b0011_1111U) << 8) + this.ReadU1();

            return ((first & 0b0001_1111U) << 24) + ((uint)this.ReadU1() << 16) + ((uint)this.ReadU1() << 8) + this.ReadU1();
        }

        public int ReadCompressedI4() {
            var size = this.buffer[this.Position] & 0xC0;
            var unsigned = (int)this.ReadCompressedU4();
            var shifted = unsigned >> 1;

            if ((unsigned & 1) == 0)
                return shifted;

            return shifted - (size == 0x00 || size == 0x40 ? 0x40 : (size == 0x80 ? 0x2000 : 0x10000000));
        }

        public T ReadEnum<T>() {
            var type = Enum.GetUnderlyingType(typeof(T));

            if (type == typeof(byte)) return (T)(object)this.ReadU1();
            else if (type == typeof(ushort)) return (T)(object)this.ReadU2();
            else if (type == typeof(uint)) return (T)(object)this.ReadU4();
            else if (type == typeof(ulong)) return (T)(object)this.ReadU8();
            else if (type == typeof(sbyte)) return (T)(object)this.ReadI1();
            else if (type == typeof(short)) return (T)(object)this.ReadI2();
            else if (type == typeof(int)) return (T)(object)this.ReadI4();
            else if (type == typeof(long)) return (T)(object)this.ReadI8();
            else throw new NotSupportedException();
        }

        public void Read(out byte value) => value = this.ReadU1();
        public void Read(out ushort value) => value = this.ReadU2();
        public void Read(out uint value) => value = this.ReadU4();
        public void Read(out ulong value) => value = this.ReadU8();
        public void Read(out sbyte value) => value = this.ReadI1();
        public void Read(out short value) => value = this.ReadI2();
        public void Read(out int value) => value = this.ReadI4();
        public void Read(out long value) => value = this.ReadI8();
        public void Read(out float value) => value = this.ReadR4();
        public void Read(out double value) => value = this.ReadR8();
        public void ReadCompressed(out uint value) => value = this.ReadCompressedU4();
        public void ReadCompressed(out int value) => value = this.ReadCompressedI4();
        public void ReadEnum<T>(out T value) => value = this.ReadEnum<T>();

        public string ReadString(Encoding encoding, uint length) => length <= int.MaxValue ? this.ReadString(encoding, (int)length) : throw new ArgumentOutOfRangeException(nameof(length));

        public string ReadString(Encoding encoding, int length) {
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));

            var end = this.Position + length;

            while (this.buffer[--end] == 0) { }

            var result = encoding.GetString(this.buffer, this.Position, end + 1 - this.Position);

            this.Position += length;

            return result;
        }

        public string ReadStringTerminated(Encoding encoding) => this.ReadStringTerminated(encoding, 0, 1);
        public string ReadStringTerminated(Encoding encoding, byte terminator) => this.ReadStringTerminated(encoding, terminator, 1);

        public string ReadStringTerminated(Encoding encoding, byte terminator, int paddingMultiple) {
            var start = this.Position;

            while (this.buffer[this.Position++] != terminator) { }

            var result = encoding.GetString(this.buffer, start, this.Position - 1 - start);

            this.Position = paddingMultiple * ((this.Position + (paddingMultiple - 1)) / paddingMultiple);

            return result;
        }

        public ByteReader ReadByteReader(uint count) => count <= int.MaxValue ? this.ReadByteReader((int)count) : throw new ArgumentOutOfRangeException(nameof(count));
        public ByteReader ReadByteReader(int count) => count >= 0 ? new ByteReader(this.ReadArray<byte>(count)) : throw new ArgumentOutOfRangeException(nameof(count));

        public void ReadArray<T>(uint count, out T[] values) => values = this.ReadArray<T>(count);
        public void ReadArray<T>(int count, out T[] values) => values = this.ReadArray<T>(count);

        public T[] ReadArray<T>(uint count) => count <= int.MaxValue ? this.ReadArray<T>((int)count) : throw new ArgumentOutOfRangeException(nameof(count));

        public T[] ReadArray<T>(int count) {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            var size = Marshal.SizeOf(default(T));
            var bytes = count * size;
            var buf = new T[count];

            Buffer.BlockCopy(this.buffer, this.Position, buf, 0, bytes);

            this.Position += bytes;

            return buf;
        }

        public void ReadCustom<T>(uint count, out T[] values) where T : ICustomByteReader, new() => values = this.ReadCustom<T>(count);
        public void ReadCustom<T>(int count, out T[] values) where T : ICustomByteReader, new() => values = this.ReadCustom<T>(count);
        public void ReadCustom<T>(out T value) where T : ICustomByteReader, new() => value = this.ReadCustom<T>();

        public void ReadCustom<T, U>(uint count, out T[] values) where T : ICustomByteReader<U>, new() where U : ByteReader => values = this.ReadCustom<T, U>(count);
        public void ReadCustom<T, U>(int count, out T[] values) where T : ICustomByteReader<U>, new() where U : ByteReader => values = this.ReadCustom<T, U>(count);
        public void ReadCustom<T, U>(out T value) where T : ICustomByteReader<U>, new() where U : ByteReader => value = this.ReadCustom<T, U>();

        public T[] ReadCustom<T>(uint count) where T : ICustomByteReader, new() => this.ReadCustom<T, ByteReader>(count);
        public T[] ReadCustom<T>(int count) where T : ICustomByteReader, new() => this.ReadCustom<T, ByteReader>(count);
        public T ReadCustom<T>() where T : ICustomByteReader, new() => this.ReadCustom<T, ByteReader>();

        public T[] ReadCustom<T, U>(int count) where T : ICustomByteReader<U>, new() where U : ByteReader => count >= 0 ? this.ReadCustom<T, U>((uint)count) : throw new ArgumentOutOfRangeException(nameof(count));

        public T[] ReadCustom<T, U>(uint count) where T : ICustomByteReader<U>, new() where U : ByteReader {
            var result = new T[count];

            for (var i = 0; i < count; i++)
                result[i] = this.ReadCustom<T, U>();

            return result;
        }

        public T ReadCustom<T, U>() where T : ICustomByteReader<U>, new() where U : ByteReader {
            var result = new T();

            result.Read((U)this);

            return result;
        }

        public T[] ReadStruct<T>(int count) where T : struct => count >= 0 ? this.ReadStruct<T>((uint)count) : throw new ArgumentOutOfRangeException(nameof(count));

        public T[] ReadStruct<T>(uint count) where T : struct {
            var result = new T[count];

            for (var i = 0; i < count; i++)
                result[i] = this.ReadStruct<T>();

            return result;
        }

        public T ReadStruct<T>() where T : struct {
            var size = Marshal.SizeOf(default(T));
            var ptr = IntPtr.Zero;

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
    }

    public interface ICustomByteReader : ICustomByteReader<ByteReader> { }

    public interface ICustomByteReader<in T> {
        void Read(T reader);
    }
}
