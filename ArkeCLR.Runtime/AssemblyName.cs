using System;
using System.Globalization;

namespace ArkeCLR.Runtime {
    public struct AssemblyName {
        public string Name { get; }
        public Version Version { get; }
        public CultureInfo Culture { get; }
        public string PublicKeyToken { get; }
        public string HintPath { get; }

        public AssemblyName(string name) : this(name, default(string)) { }
        public AssemblyName(string name, string hintPath) : this(name, default(string), default(string), default(string), hintPath) { }
        public AssemblyName(string name, string version, string culture, string publicKeyToken) : this(name, version, culture, publicKeyToken, default(string)) { }
        public AssemblyName(string name, string version, string culture, string publicKeyToken, string hintPath) : this(name, version != null ? Version.Parse(version) : new Version(0, 0, 0, 0), culture != null ? new CultureInfo(culture) : CultureInfo.InvariantCulture, publicKeyToken, hintPath) { }
        public AssemblyName(string name, Version version, CultureInfo culture, string publicKeyToken, string hintPath) => (this.Name, this.Version, this.Culture, this.PublicKeyToken, this.HintPath) = (name, version, culture, publicKeyToken, hintPath);

        public string FullName => $"{this.Name}, Version={this.Version}, Culture={this.Culture.Name}, PublicKeyToken={(this.PublicKeyToken ?? "null")}";

        public override string ToString() => this.FullName;
    }
}
