using System;
using System.Globalization;

namespace ArkeCLR.Runtime {
    public struct AssemblyName {
        public string Name { get; set; }
        public Version Version { get; set; }
        public CultureInfo Culture { get; set; }
        public string PublicKeyToken { get; set; }

        public AssemblyName(string name) : this(name, default(string), default(string), default(string)) { }
        public AssemblyName(string name, string version, string culture, string publicKeyToken) : this(name, version != null ? Version.Parse(version) : null, culture != null ? new CultureInfo(culture) : null, publicKeyToken) { }

        public AssemblyName(string name, Version version, CultureInfo culture, string publicKeyToken) {
            this.Name = name;
            this.Version = version;
            this.Culture = culture;
            this.PublicKeyToken = publicKeyToken;
        }

        public string FullName {
            get {
                var result = this.Name;

                result += ", Version=" + (this.Version ?? new Version(0, 0, 0, 0));
                result += ", Culture=" + (this.Culture?.Name ?? "neutral");
                result += ", PublicKeyToken=" + (this.PublicKeyToken ?? "null");

                return result;
            }
        }
    }
}
