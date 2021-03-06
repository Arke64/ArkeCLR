﻿using ArkeCLR.Utilities;
using System;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Signatures {
    public class FieldSig : ICustomByteReader {
        public CustomMod[] CustomMods;
        public Type Type;

        public void Read(ByteReader reader) {
            if (!reader.TryReadEnum(SignatureFlags.Field)) throw new InvalidOperationException();

            this.CustomMods = CustomMod.ReadCustomMods(reader);

            reader.ReadCustom(out this.Type);
        }
    }
}
