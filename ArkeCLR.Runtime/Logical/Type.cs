using ArkeCLR.Runtime.Files;
using ArkeCLR.Utilities.Extensions;
using System.Collections.Generic;
using System.Linq;
using ArkeCLR.Runtime.Tables;

namespace ArkeCLR.Runtime.Logical {
    public class Type {
        public Assembly Assembly { get; }
        public uint Row { get; }
        public string Name { get; }
        public string Namespace { get; }
        public IReadOnlyCollection<Method> Methods { get; }

        public Type(CliFile file, Assembly assembly, TypeDef def, uint row) {
            this.Assembly = assembly;
            this.Row = row;
            this.Name = file.StringStream.GetAt(def.TypeName);
            this.Namespace = file.StringStream.GetAt(def.TypeNamespace);
            this.Methods = file.TableStream.ExtractMethodList(def, row).ToList(d => new Method(file, this, d.def, d.row));
        }
    }
}
