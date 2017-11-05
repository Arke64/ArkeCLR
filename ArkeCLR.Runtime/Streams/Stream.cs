using ArkeCLR.Utilities;

namespace ArkeCLR.Runtime.Streams {
    public abstract class Stream {
        public string Name { get; }

        protected Stream(string name) => this.Name = name;

        public abstract void Initialize(ByteReader reader);
    }
}
