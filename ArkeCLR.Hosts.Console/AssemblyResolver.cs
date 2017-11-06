using ArkeCLR.Runtime.Execution;
using System.IO;

namespace ArkeCLR.Hosts.Console {
    public class AssemblyResolver : IAssemblyResolver {
        private readonly string currentDirectory;

        public AssemblyResolver(string currentDirectory) => this.currentDirectory = currentDirectory;

        public bool TryResolve(AssemblyName assemblyName, out byte[] result) {
            var path = default(string);
            var root = Path.Combine(this.currentDirectory, assemblyName.Name);

            result = default;

            if (File.Exists(assemblyName.HintPath)) {
                path = assemblyName.HintPath;
            }
            else if (File.Exists(root + ".dll")) {
                path = root + ".dll";
            }
            else if (File.Exists(root + ".exe")) {
                path = root + ".exe";
            }
            else {
                return false;
            }

            result = File.ReadAllBytes(path);

            return true;
        }
    }
}
