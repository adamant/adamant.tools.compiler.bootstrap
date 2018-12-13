using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public abstract class SimpleType : ValueType
    {
        public Name Name { get; }
        public override ValueSemantics Semantics => ValueSemantics.Copy;

        protected SimpleType(string name)
        {
            Name = SimpleName.Special(name);
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
