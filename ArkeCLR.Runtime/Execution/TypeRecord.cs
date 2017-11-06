using ArkeCLR.Runtime.Signatures;
using System.Runtime.InteropServices;
using System;

namespace ArkeCLR.Runtime.Execution {
    [StructLayout(LayoutKind.Explicit)]
    public struct TypeRecord {
        [FieldOffset(0)]
        public ElementType Tag;
        [FieldOffset(4)]
        public byte U1;
        [FieldOffset(4)]
        public ushort U2;
        [FieldOffset(4)]
        public uint U4;
        [FieldOffset(4)]
        public ulong U8;
        [FieldOffset(4)]
        public sbyte I1;
        [FieldOffset(4)]
        public short I2;
        [FieldOffset(4)]
        public int I4;
        [FieldOffset(4)]
        public long I8;
        [FieldOffset(4)]
        public float R4;
        [FieldOffset(4)]
        public double R8;
        [FieldOffset(4)]
        public bool Boolean;
        [FieldOffset(4)]
        public char Char;
        [FieldOffset(4)]
        public long I;
        [FieldOffset(4)]
        public ulong U;

        //TODO What type should these actually be?
        [FieldOffset(4)]
        public ulong TypedByRef;
        [FieldOffset(4)]
        public ulong String;
        [FieldOffset(4)]
        public ulong Object;

        public static TypeRecord FromU1(byte value) => new TypeRecord { Tag = ElementType.U1, U1 = value };
        public static TypeRecord FromU2(ushort value) => new TypeRecord { Tag = ElementType.U2, U2 = value };
        public static TypeRecord FromU4(uint value) => new TypeRecord { Tag = ElementType.U4, U4 = value };
        public static TypeRecord FromU8(ulong value) => new TypeRecord { Tag = ElementType.U8, U8 = value };
        public static TypeRecord FromI1(sbyte value) => new TypeRecord { Tag = ElementType.I1, I1 = value };
        public static TypeRecord FromI2(short value) => new TypeRecord { Tag = ElementType.I2, I2 = value };
        public static TypeRecord FromI4(int value) => new TypeRecord { Tag = ElementType.I4, I4 = value };
        public static TypeRecord FromI8(long value) => new TypeRecord { Tag = ElementType.I8, I8 = value };
        public static TypeRecord FromR4(float value) => new TypeRecord { Tag = ElementType.R4, R4 = value };
        public static TypeRecord FromR8(double value) => new TypeRecord { Tag = ElementType.R8, R8 = value };
        public static TypeRecord FromBoolean(bool value) => new TypeRecord { Tag = ElementType.Boolean, Boolean = value };
        public static TypeRecord FromChar(char value) => new TypeRecord { Tag = ElementType.Char, Char = value };
        public static TypeRecord FromI(long value) => new TypeRecord { Tag = ElementType.I, I = value };
        public static TypeRecord FromU(ulong value) => new TypeRecord { Tag = ElementType.U, U = value };
        public static TypeRecord FromTypedByRef(ulong value) => new TypeRecord { Tag = ElementType.TypedByRef, TypedByRef = value };
        public static TypeRecord FromString(ulong value) => new TypeRecord { Tag = ElementType.String, String = value };
        public static TypeRecord FromObject(ulong value) => new TypeRecord { Tag = ElementType.Object, Object = value };

        //TODO Need to properly implement what CLR types can be added, also overflow
        public static TypeRecord Add(TypeRecord a, TypeRecord b) {
            switch (a.Tag) {
                case ElementType.I4:
                    switch (b.Tag) {
                        case ElementType.I4:
                            return TypeRecord.FromI4(a.I4 + b.I4);
                    }

                    break;
            }

            throw new NotImplementedException();
        }
    }
}
