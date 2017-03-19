using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Runtime.Logical {
    public class Assembly {
        public CliFile File { get; }
        public string Name { get; }
        public Method EntryPoint { get; }
        public IReadOnlyList<Type> Types { get; }
        public IReadOnlyList<Method> Methods { get; }

        public Assembly(CliFile file) {
            var def = file.TableStream.Assemblies.Single();

            this.File = file;
            this.Name = file.StringStream.GetAt(def.Name);
            this.Types = file.TableStream.TypeDefs.ToList((d, i) => new Type(file, this, d, (uint)i));

            this.Methods = this.Types.SelectMany(t => t.Methods).OrderBy(m => m.Row).ToList();
            this.EntryPoint = this.FindMethod(new TableIndex(file.CliHeader.EntryPointToken));
        }

        public Method FindMethod(TableIndex index) => index.Table == TableType.MethodDef ? this.Methods[(int)index.Row - 1] : null; //TODO Need to handle methodref
    }
}
