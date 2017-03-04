using ArkeCLR.Runtime;
using System;
using System.Diagnostics;
using System.IO;

namespace ArkeCLR.Hosts.Console {
    public class Program {
        public static void Main(string[] args) {
            void log(string message) => System.Console.WriteLine(message);

            if (args.Length != 1) {
                log("Usage: ArkeCLR.Hosts.Console [entry point]");

                return;
            }

            var host = new Host(new AssemblyResolver(Path.GetDirectoryName(args[0])), log);

            try {
                log($"Exited with code {host.Run(args[0])}.");
            }
            catch (Exception ex) {
                log(ex.ToString());

                Debugger.Break();
            }
        }
    }
}