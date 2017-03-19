using System;
using ArkeCLR.Utilities;
using System.Linq;
using System.Collections.Generic;
using ArkeCLR.Utilities.Extensions;

namespace ArkeCLR.Runtime.Signatures {
    public struct LocalVarSig : ICustomByteReader {
        public uint Count;
        public LocalVar[] Locals;

        public void Read(ByteReader reader) {
            var sig = reader.ReadU1();

            if (sig != 0x07) throw new InvalidOperationException();

            reader.ReadCompressed(out this.Count);

            this.Locals = new LocalVar[this.Count];
            for (var i = 0; i < this.Count; i++)
                this.Locals[i].Read(reader);
        }

        public struct LocalVar {
            public LocalVarCustomMod[] CustomMods;
            public bool IsByRef;
            public bool IsTypedByRef;
            public Type Type;

            public struct LocalVarCustomMod {
                public CustomMod CustomMod;
                public Constraint Constraint;
            }

            public void Read(ByteReader reader) {
                var cur = reader.ReadEnum<ElementType>();
                var mods = new List<LocalVarCustomMod>();

                if (cur != ElementType.TypedByRef) {
                    while (cur == ElementType.CModOpt || cur == ElementType.CModReqD || cur == ElementType.Pinned) {
                        if (cur == ElementType.CModOpt || cur == ElementType.CModReqD) {
                            var mod = new CustomMod();
                            var cons = new Constraint();

                            mod.Read(cur, reader);

                            cur = reader.ReadEnum<ElementType>();

                            if (cur == ElementType.Pinned) {
                                cons.Type = ElementType.Pinned;

                                cur = reader.ReadEnum<ElementType>();
                            }

                            mods.Add(new LocalVarCustomMod { CustomMod = mod, Constraint = cons });
                        }
                        else {
                            mods.Add(new LocalVarCustomMod { Constraint = new Constraint { Type = ElementType.Pinned } });

                            cur = reader.ReadEnum<ElementType>();
                        }
                    }

                    if (cur == ElementType.ByRef) {
                        this.IsByRef = true;

                        cur = reader.ReadEnum<ElementType>();
                    }

                    this.Type.Read(cur, reader);
                }
                else {
                    this.IsTypedByRef = true;
                }

                this.CustomMods = mods.ToArray();
            }

            public override string ToString() => $"{this.CustomMods.ToString(", ", "[", "] ", true)}{(!this.IsTypedByRef ? this.Type.ToString() : "typedref")}{(this.IsByRef ? "&" : string.Empty)}";
        }

        public override string ToString() => $"<{string.Join(" ", this.Locals.Select(t => t.ToString()))}>";
    }
}
