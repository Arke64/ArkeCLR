using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ArkeCLR.Runtime.Execution {
    public struct AssemblyName {
        public string Name { get; }
        public Version Version { get; }
        public CultureInfo Culture { get; }
        public byte[] PublicKeyToken { get; }
        public string HintPath { get; }

        public AssemblyName(string name) : this(name, default) { }
        public AssemblyName(string name, string hintPath) : this(name, default, default(string), default, hintPath) { }
        public AssemblyName(string name, string version, string culture, byte[] publicKeyToken) : this(name, version, culture, publicKeyToken, default) { }
        public AssemblyName(string name, string version, string culture, byte[] publicKeyToken, string hintPath) : this(name, version != null ? Version.Parse(version) : new Version(0, 0, 0, 0), culture != null ? new CultureInfo(culture) : CultureInfo.InvariantCulture, publicKeyToken, hintPath) { }
        public AssemblyName(string name, Version version, CultureInfo culture, byte[] publicKeyToken) : this(name, version, culture, publicKeyToken, default) { }
        public AssemblyName(string name, Version version, CultureInfo culture, byte[] publicKeyToken, string hintPath) => (this.Name, this.Version, this.Culture, this.PublicKeyToken, this.HintPath) = (name, version, culture, publicKeyToken, hintPath);

        public static AssemblyName FromFilePath(string filePath) => new AssemblyName(Path.GetFileNameWithoutExtension(filePath), filePath);

        public string FullName => $"{this.Name}, Version={this.Version}, Culture={this.Culture.Name}, PublicKeyToken={(AssemblyName.FormatPublicKeyToken(this.PublicKeyToken))}";

        private static string FormatPublicKeyToken(byte[] token) => token != null ? string.Join(string.Empty, token.Select(b => b.ToString("X2"))) : new string('0', 16);
    }
}
