using ArkeCLR.Utilities;
using System;
using System.Collections.Generic;

#pragma warning disable S1104

namespace ArkeCLR.Runtime.Signatures {
    public class LocalVarSig : ICustomByteReader {
        public uint Count;
        public LocalVar[] Locals;

        public void Read(ByteReader reader) {
            if (!reader.TryReadEnum(SignatureFlags.LocalSig)) throw new InvalidOperationException();

            reader.ReadCompressed(out this.Count);
            reader.ReadCustom(this.Count, out this.Locals);
        }

        public class LocalVar : ICustomByteReader {
            public CustomMod[] CustomMods;
            public Constraint[] Constraints;
            public bool IsByRef;
            public bool IsTypedByRef;
            public Type Type;

            public void Read(ByteReader reader) {
                this.IsTypedByRef = reader.TryReadEnum(ElementType.TypedByRef);

                var mods = new List<CustomMod>();
                var cons = new List<Constraint>();

                if (!this.IsTypedByRef) {
                    while (reader.TryPeekEnum(out var res, ElementType.CModOpt, ElementType.CModReqD, ElementType.Pinned)) {
                        if (res == ElementType.Pinned) { cons.Add(reader.ReadCustom<Constraint>()); }
                        else { mods.Add(reader.ReadCustom<CustomMod>()); }
                    }

                    this.IsByRef = reader.TryReadEnum(ElementType.ByRef);

                    reader.ReadCustom(out this.Type);
                }

                this.CustomMods = mods.ToArray();
                this.Constraints = cons.ToArray();
            }
        }
    }
}
