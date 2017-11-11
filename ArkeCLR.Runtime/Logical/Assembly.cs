using ArkeCLR.Runtime.Execution;
using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Streams;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ArkeCLR.Runtime.Logical {
    public class Assembly {
        public CliFile CliFile { get; }
        public AssemblyName Name { get; }
        public IReadOnlyCollection<Type> Types { get; }

        public Assembly(CliFile file) {
            var def = file.TableStream.Assemblies.Get(new TableToken { Row = 1, Table = TableType.Assembly });

            this.CliFile = file;
            this.Name = new AssemblyName(file.StringStream.GetAt(def.Name), new Version(def.MajorVersion, def.MinorVersion, def.BuildNumber, def.RevisionNumber), new CultureInfo(file.StringStream.GetAt(def.Culture)), file.BlobStream.GetAt(def.PublicKey));
            this.Types = file.TableStream.TypeDefs.ExtractRun(file.TableStream.Assemblies, p => 1, def, 1, (d, r) => new Type(file, this, d, r));
        }
    }
}
