﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ArkeCLR.Utilities {
    public class ByteReader { 
        private readonly byte[] buffer;

        public int Position { get; private set; }
        public int Length { get; }

        protected ByteReader(ByteReader reader) => this.buffer = reader.buffer;

        public ByteReader(byte[] buffer) : this(buffer, 0, buffer?.Length ?? 0) { }

        public ByteReader(byte[] buffer, int offset, int size) {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (size <= 0 || offset + size > buffer.Length) throw new ArgumentOutOfRangeException(nameof(size));

            this.buffer = new byte[size];

            Array.Copy(buffer, offset, this.buffer, 0, size);

            this.Length = size;
            this.Position = 0;
        }

        public void Seek(uint position, SeekOrigin seekOrigin) => this.Seek((int)position, seekOrigin);

        public void Seek(int position, SeekOrigin seekOrigin) {
            if (seekOrigin == SeekOrigin.Begin) {
                this.Position = position;
            }
            else if (seekOrigin == SeekOrigin.Current) {
                this.Position += position;
            }
            else if (seekOrigin == SeekOrigin.End) {
                this.Position = this.Length + position;
            }
        }

        public ByteReader CreateView(int offset, int size) => new ByteReader(this.buffer, offset, size);
        public ByteReader CreateView(uint offset, uint size) => this.CreateView((int)offset, (int)size);

        public byte ReadU1() => this.buffer[this.Position++];
        public ushort ReadU2() { var r = BitConverter.ToUInt16(this.buffer, this.Position); this.Position += sizeof(ushort); return r; }
        public uint ReadU4() { var r = BitConverter.ToUInt32(this.buffer, this.Position); this.Position += sizeof(uint); return r; }
        public ulong ReadU8() { var r = BitConverter.ToUInt64(this.buffer, this.Position); this.Position += sizeof(ulong); return r; }
        public sbyte ReadI1() => (sbyte)this.buffer[this.Position++];
        public short ReadI2() { var r = BitConverter.ToInt16(this.buffer, this.Position); this.Position += sizeof(short); return r; }
        public int ReadI4() { var r = BitConverter.ToInt32(this.buffer, this.Position); this.Position += sizeof(int); return r; }
        public long ReadI8() { var r = BitConverter.ToInt64(this.buffer, this.Position); this.Position += sizeof(long); return r; }

        public string ReadString(Encoding encoding, uint length) => this.ReadString(encoding, (int)length, 0);
        public string ReadString(Encoding encoding, uint maxLength, byte padder) => this.ReadString(encoding, (int)maxLength, padder);

        public string ReadString(Encoding encoding, int length) => this.ReadString(encoding, length, 0);

        public string ReadString(Encoding encoding, int maxLength, byte padder) {
            var end = this.Position + maxLength;

            while (this.buffer[--end] == padder)
                ;

            var result = encoding.GetString(this.buffer, this.Position, end + 1 - this.Position);

            this.Position += maxLength;

            return result;
        }

        public string ReadStringTerminated(Encoding encoding) => this.ReadStringTerminated(encoding, 0, 1);
        public string ReadStringTerminated(Encoding encoding, byte terminator) => this.ReadStringTerminated(encoding, terminator, 1);

        public string ReadStringTerminated(Encoding encoding, byte terminator, int paddingMultiple) {
            var start = this.Position;

            while (this.buffer[this.Position++] != terminator)
                ;

            var result = encoding.GetString(this.buffer, start, this.Position - 1 - start);

            this.Position = paddingMultiple * ((this.Position + (paddingMultiple - 1)) / paddingMultiple);

            return result;
        }

        public T[] ReadArray<T>(uint count) => this.ReadArray<T>((int)count);

        public T[] ReadArray<T>(int count) {
            var size = Marshal.SizeOf(default(T));
            var bytes = count * size;
            var buffer = new T[count];

            Buffer.BlockCopy(this.buffer, this.Position, buffer, 0, bytes);

            this.Position += bytes;

            return buffer;
        }

        //TODO Find some better way of doing this so we don't need to pass in the derived reader
        public T[] ReadCustom<T>(uint count) where T : struct, ICustomByteReader => this.ReadCustom<T, ByteReader>(count, this);
        public T[] ReadCustom<T>(int count) where T : struct, ICustomByteReader => this.ReadCustom<T, ByteReader>(count, this);
        public T ReadCustom<T>() where T : struct, ICustomByteReader => this.ReadCustom<T, ByteReader>(this);

        public T[] ReadCustom<T, U>(uint count, U reader) where T : struct, ICustomByteReader<U> => this.ReadCustom<T, U>((int)count, reader);

        public T[] ReadCustom<T, U>(int count, U reader) where T : struct, ICustomByteReader<U> {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            var result = new T[count];

            for (var i = 0; i < count; i++)
                result[i] = this.ReadCustom<T, U>(reader);

            return result;
        }

        public T ReadCustom<T, U>(U reader) where T : struct, ICustomByteReader<U> {
            var result = new T();

            result.Read(reader);

            return result;
        }

        public T[] ReadStruct<T>(uint count) where T : struct => this.ReadStruct<T>((int)count);

        public T[] ReadStruct<T>(int count) where T : struct {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

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

    public interface ICustomByteReader<T> {
        void Read(T reader);
    }
}
