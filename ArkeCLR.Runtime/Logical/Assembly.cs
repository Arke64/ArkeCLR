using ArkeCLR.Runtime.Files;
using ArkeCLR.Utilities.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Runtime.Logical {
    public class Assembly {
        public string Name { get; }
        public IReadOnlyCollection<Method> Methods { get; }

        public Assembly(CliFile file) {
            var def = file.TableStream.Assemblies.Single();

            this.Name = file.StringStream.GetAt(def.Name);
            this.Methods = file.TableStream.MethodDefs.ToList(d => new Method(file, d));
        }
    }
}
