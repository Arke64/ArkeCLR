using ArkeCLR.Runtime.Execution;
using System.IO;

namespace ArkeCLR.Hosts.Console {
    public class AssemblyResolver : IAssemblyResolver {
        private readonly string currentDirectory;

        public AssemblyResolver(string currentDirectory) => this.currentDirectory = currentDirectory;

        public (bool, byte[]) Resolve(AssemblyName assemblyName) {
            var path = default(string);
            var root = Path.Combine(this.currentDirectory, assemblyName.Name);

            if (File.Exists(assemblyName.HintPath)) {
                path = assemblyName.HintPath;
            }
            else if (File.Exists(root + ".dll")) {
                path = root + ".dll";
            }
            else if (File.Exists(root + ".exe")) {
                path = root + ".exe";
            }

            return path != default(string) ? (true, File.ReadAllBytes(path)) : (false, null);
        }
    }
}