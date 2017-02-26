using ArkeCLR.Runtime.Flags;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Tables {
    public struct AssemblyRef : ICustomByteReader<TableStreamReader> {
        public ushort MajorVersion;
        public ushort MinorVersion;
        public ushort BuildNumber;
        public ushort RevisionNumber;
        public AssemblyFlags Flags;
        public uint PublicKeyOrToken;
        public uint Name;
        public uint Culture;
        public uint HashValue;

        public void Read(TableStreamReader reader) {
            reader.Read(ref this.MajorVersion);
            reader.Read(ref this.MinorVersion);
            reader.Read(ref this.BuildNumber);
            reader.Read(ref this.RevisionNumber);
            reader.ReadEnum(ref this.Flags);
            reader.Read(ref this.PublicKeyOrToken, HeapType.Blob);
            reader.Read(ref this.Name, HeapType.String);
            reader.Read(ref this.Culture, HeapType.String);
            reader.Read(ref this.HashValue, HeapType.Blob);
        }
    }
}
