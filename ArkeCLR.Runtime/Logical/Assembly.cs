using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Streams;
using ArkeCLR.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArkeCLR.Runtime.Logical {
    public class Assembly {
        public CliFile File { get; }
        public string Name { get; }
        public Method EntryPoint { get; }
        public IReadOnlyCollection<Type> Types { get; }
        public IReadOnlyCollection<Method> Methods { get; }

        public Assembly(CliFile file) {
            var def = file.TableStream.Assemblies.Get(new TableIndex { Row = 1, Table = TableType.Assembly });

            this.File = file;
            this.Name = file.StringStream.GetAt(def.Name);
            this.Types = file.TableStream.TypeDefs.ExtractRun(file.TableStream.Assemblies, p => 1, def, 1, (d, r) => new Type(file, this, d, r));

            this.Methods = this.Types.SelectMany(t => t.Methods).OrderBy(m => m.Row).ToList();
            this.EntryPoint = this.FindMethod(file.TableStream.ToTableIndex(file.CliHeader.EntryPointToken));
        }

        public Method FindMethod(TableIndex index) => index.Table == TableType.MethodDef ? this.Methods.Skip((int)index.Row - 1).First() : throw new NotImplementedException();
    }
}
