using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    [Closed(
        typeof(BoolType),
        typeof(NumericType))]
    public abstract class SimpleType : ValueType
    {
        public Name Name { get; }

        public override TypeSemantics Semantics => TypeSemantics.Copy;

        public override ExpressionSemantics OldValueSemantics => ExpressionSemantics.Copy;

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
