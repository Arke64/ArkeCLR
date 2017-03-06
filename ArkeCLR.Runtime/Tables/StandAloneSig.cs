﻿using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct StandAloneSig : ICustomByteReader<TableStreamReader> {
        public HeapIndex Signature;

        public void Read(TableStreamReader reader) => reader.ReadCustom(out this.Signature);
    }
}