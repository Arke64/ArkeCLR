using System;

namespace ArkeCLR.Runtime.Tables.Flags {
    [Flags]
    public enum PInvokeAttributes : ushort {
        NoMangle = 0x0001,

        CharSetMask = 0x0006,
        CharSetNotSpec = 0x0000,
        CharSetAnsi = 0x0002,
        CharSetUnicode = 0x0004,
        CharSetAuto = 0x0006,

        SupportsLastError = 0x0040,

        CallConvMask = 0x0700,
        CallConvPlatformapi = 0x0100,
        CallConvCdecl = 0x0200,
        CallConvStdCall = 0x0300,
        CallConvThiscall = 0x0400,
        CallConvFastcall = 0x0500,
    }
}
