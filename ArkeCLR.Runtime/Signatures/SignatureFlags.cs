using System;

namespace ArkeCLR.Runtime.Signatures {
    [Flags]
    public enum SignatureFlags : byte {
        CallingConventionMask = 0x0F,
        Default = 0x00,
        C = 0x01,
        StdCall = 0x02,
        ThisCall = 0x03,
        FastCall = 0x04,
        VarArg = 0x05,

        Field = 0x06,
        LocalSig = 0x07,
        Property = 0x08,
        GenericInst = 0x0A,

        Generic = 0x10,
        HasThis = 0x20,
        ExplicitThis = 0x40,
    }
}
