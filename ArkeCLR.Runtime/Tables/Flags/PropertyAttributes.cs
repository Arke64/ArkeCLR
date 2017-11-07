using System;

namespace ArkeCLR.Runtime.Tables.Flags {
    [Flags]
    public enum PropertyAttributes : ushort {
        SpecialName = 0x0200,
        RTSpecialName = 0x0400,
        HasDefault = 0x1000,
        Unused = 0xE9F
    }
}
