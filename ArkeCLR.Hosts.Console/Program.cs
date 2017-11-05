using ArkeCLR.Runtime.Execution;
using System;
using System.Diagnostics;
using System.IO;

namespace ArkeCLR.Hosts.Console {
    public static class Program {
        public static void Main(string[] args) {
            void log(string message) => System.Console.WriteLine(message);

            if (args.Length != 1) {
                log("Usage: ArkeCLR.Hosts.Console [entry point]");

                return;
            }

            try {
                log($"Exited with code {new ExecutionHost(new Interpreter(), new AssemblyResolver(Path.GetDirectoryName(args[0])), log).Run(args[0])}.");
            }
            catch (Exception ex) {
                log(ex.ToString());

                Debugger.Break();
            }
        }
    }
}
