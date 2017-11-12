using ArkeCLR.Runtime.Execution;
using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Streams;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ArkeCLR.Runtime.Logical {
    public class Assembly {
        public CliFile CliFile { get; }
        public AssemblyName Name { get; }
        public IReadOnlyCollection<Type> Types { get; }
        public Method EntryPoint { get; }

        public Assembly(IFileResolver fileResolver, CliFile file) {
            var def = file.TableStream.Assemblies.Get(new TableToken(TableType.Assembly, 1));

            //TODO Need to resolve other modules and files in this logical assembly. It's expected that this CliFile is the manifest assembly. See II.6.1.

            this.CliFile = file;
            this.Name = new AssemblyName(file.StringStream.GetAt(def.Name), new Version(def.MajorVersion, def.MinorVersion, def.BuildNumber, def.RevisionNumber), new CultureInfo(file.StringStream.GetAt(def.Culture)), file.BlobStream.GetAt(def.PublicKey));
            this.Types = file.TableStream.TypeDefs.ExtractRun(file.TableStream.Assemblies, p => 1, def, 1, (d, r) => new Type(file, this, d, r));

            var token = new TableToken(this.CliFile.CliHeader.EntryPointToken);
            if (!token.IsZero)
                this.EntryPoint = this.Types.SelectMany(t => t.Methods).Single(m => m.Row == token.Row);
        }
    }
}
