namespace ArkeCLR.Runtime.Signatures {
    public enum CallingConvention : byte {
        Default = 0x00,
        C = 0x01,
        StdCall = 0x02,
        ThisCall = 0x03,
        FastCall = 0x04,
        VarArg = 0x05,
        Generic = 0x10,
    }
}
