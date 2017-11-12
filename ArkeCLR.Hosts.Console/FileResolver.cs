using ArkeCLR.Runtime.Execution;
using System.IO;

namespace ArkeCLR.Hosts.Console {
    public class FileResolver : IFileResolver {
        private readonly string[] searchDirectories;

        public FileResolver(params string[] searchDirectories) => this.searchDirectories = searchDirectories;

        public bool TryResolve(string fileName, string hintPath, out byte[] result) {
            result = default;

            foreach (var d in this.searchDirectories) {
                var path = default(string);
                var root = Path.Combine(d, fileName);

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
                    continue;
                }

                result = File.ReadAllBytes(path);

                return true;
            }

            return false;
        }
    }
}
