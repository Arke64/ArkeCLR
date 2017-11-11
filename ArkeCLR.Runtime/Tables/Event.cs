﻿using ArkeCLR.Runtime.Tables.Flags;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct Event : ICustomByteReader<TokenByteReader> {
        public EventAttributes EventFlags;
        public HeapToken Name;
        public TableToken EventType;

        public void Read(TokenByteReader reader) {
            reader.ReadEnum(out this.EventFlags);
            reader.Read(HeapType.String, out this.Name);
            reader.Read(CodedIndexType.TypeDefOrRef, out this.EventType);
        }
    }
}