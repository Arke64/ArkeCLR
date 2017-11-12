using ArkeCLR.Runtime.Files;
using ArkeCLR.Runtime.Tables;
using System.Collections.Generic;

namespace ArkeCLR.Runtime.Logical {
    public class Type {
        public Assembly Assembly { get; }
        public uint Row { get; }
        public string Name { get; }
        public string Namespace { get; }
        public IReadOnlyCollection<Method> Methods { get; }
        public IReadOnlyCollection<Field> Fields { get; }

        public Type(CliFile file, Assembly assembly, TypeDef def, uint row) {
            this.Assembly = assembly;
            this.Row = row;
            this.Name = file.StringStream.GetAt(def.TypeName);
            this.Namespace = file.StringStream.GetAt(def.TypeNamespace);
            this.Methods = file.TableStream.MethodDefs.ExtractRun(file.TableStream.TypeDefs, p => p.MethodList.Row, def, row, (d, r) => new Method(file, this, d, r));
            this.Fields = file.TableStream.Fields.ExtractRun(file.TableStream.TypeDefs, p => p.FieldList.Row, def, row, (d, r) => new Field(file, this, d, r));
        }
    }
}
