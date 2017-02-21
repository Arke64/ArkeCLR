using System;
using ArkeCLR.Utilities;
using ArkeCLR.Runtime.Streams;

namespace ArkeCLR.Runtime {
    public struct Index : ICustomByteReader<TableStream> {
        public byte Table;
        public uint Row;

        public void Read(ByteReader reader, TableStream context) => throw new NotImplementedException();
    }
}
