using ArkeCLR.Runtime.Execution;
using System.IO;

namespace ArkeCLR.Hosts.Console {
    public class FileResolver : IFileResolver {
        private readonly string currentDirectory;

        public FileResolver(string currentDirectory) => this.currentDirectory = currentDirectory;

        public bool TryResolve(string fileName, string hintPath, out byte[] result) {
            var path = default(string);
            var root = Path.Combine(this.currentDirectory, fileName);

            result = default;

            if (File.Exists(hintPath)) {
                path = hintPath;
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
