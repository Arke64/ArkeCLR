using ArkeCLR.Runtime.Files;
using ArkeCLR.Utilities.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Runtime.Logical {
    public class Assembly {
        public string Name { get; }
        public IReadOnlyCollection<Type> Types { get; }

        public Assembly(CliFile file) {
            var def = file.TableStream.Assemblies.Single();

            this.Name = file.StringStream.GetAt(def.Name);
            this.Types = file.TableStream.TypeDefs.ToList((d, i) => new Type(file, d, i));
        }
    }
}
