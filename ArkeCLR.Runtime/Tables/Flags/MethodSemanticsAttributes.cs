using System;

namespace ArkeCLR.Runtime.Tables.Flags {
    [Flags]
    public enum MethodSemanticsAttributes : ushort {
        Setter = 0x0001,
        Getter = 0x0002,
        Other = 0x0004,
        AddOn = 0x0008,
        RemoveOn = 0x0010,
        Fire = 0x0020
    }
}
