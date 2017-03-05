using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Runtime.Logical {
    public class Assembly {
        public string Name { get; }
        public Method EntryPoint { get; }
        public IReadOnlyList<Type> Types { get; }
        public IReadOnlyList<Method> Methods { get; }

        public Assembly(CliFile file) {
            var def = file.TableStream.Assemblies.Single();

            this.Name = file.StringStream.GetAt(def.Name);
            this.Types = file.TableStream.TypeDefs.ToList((d, i) => new Type(file, this, d, (uint)i));

            this.Methods = this.Types.SelectMany(t => t.Methods).OrderBy(m => m.Row).ToList();
            this.EntryPoint = this.FindMethod(TableIndex.From(file.CliHeader.EntryPointToken));
        }

        public Method FindMethod(TableIndex index) => this.Methods[(int)index.Row - 1];
    }
}
