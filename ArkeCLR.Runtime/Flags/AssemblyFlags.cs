using System;

namespace ArkeCLR.Runtime.Flags {
    [Flags]
    public enum AssemblyFlags : uint {
        PublicKey = 0x0001,
        Retargetable = 0x0100,
        DisableJITCompileOptimizer = 0x4000,
        EnableJITCompileTracking = 0x8000,
    }
}
