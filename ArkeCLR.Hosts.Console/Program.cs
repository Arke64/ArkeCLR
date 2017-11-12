using ArkeCLR.Runtime.Execution;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ArkeCLR.Hosts.Console {
    public static class Program {
        public static void Main(string[] args) {
            void log(string message) => System.Console.WriteLine(message);

            if (args.Length == 0) {
                log("Usage: ArkeCLR.Hosts.Console [entry point] [optional args]");

                return;
            }

            var fullPath = Path.GetFullPath(args[0]);

            void run() => log($"Exited with code {new ExecutionHost(new Interpreter(new FileResolver(Path.GetDirectoryName(fullPath)), log)).Run(fullPath, args.Skip(1))}.");

            if (!Debugger.IsAttached) {
                try {
                    run();
                }
                catch (Exception ex) {
                    log(ex.ToString());
                }
            }
            else {
                run();
            }
        }
    }
}
