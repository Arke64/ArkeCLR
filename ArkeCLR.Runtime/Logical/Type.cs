using ArkeCLR.Runtime.Files;
using ArkeCLR.Utilities.Extensions;
using System.Collections.Generic;
using System.Linq;
using ArkeCLR.Runtime.Tables;

namespace ArkeCLR.Runtime.Logical {
    public class Type {
        public string Name { get; }
        public string Namespace { get; }
        public IReadOnlyCollection<Method> Methods { get; }

        public Type(CliFile file, TypeDef def, int defIndex) {
            this.Name = file.StringStream.GetAt(def.TypeName);
            this.Namespace = file.StringStream.GetAt(def.TypeNamespace);
            this.Methods = file.TableStream.ExtractMethodList(def, defIndex).ToList(d => new Method(file, d));
        }
    }
}
