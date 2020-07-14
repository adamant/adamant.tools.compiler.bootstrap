using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    [Closed(
        typeof(BoolType),
        typeof(NumericType))]
    public abstract class SimpleType : ValueType
    {
        public Name Name { get; }

        public override TypeSemantics Semantics => TypeSemantics.Copy;

        private protected SimpleType(string name)
        {
            Name = SimpleName.Special(name);
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
