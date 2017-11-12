using ArkeCLR.Runtime.Files;

namespace ArkeCLR.Runtime.Logical {
    public class Field {
        public Type Type { get; }
        public uint Row { get; }
        public string Name { get; }

        public Field(CliFile file, Type type, Tables.Field def, uint row) {
            this.Type = type;
            this.Row = row;
            this.Name = file.StringStream.GetAt(def.Name);
        }
    }
}
