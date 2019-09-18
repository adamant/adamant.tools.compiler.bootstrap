using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    [Closed(
        typeof(BoolType),
        typeof(NumericType),
        typeof(StringConstantType))]
    public abstract class SimpleType : ValueType
    {
        public Name Name { get; }
        public override ValueSemantics ValueSemantics => ValueSemantics.Copy;

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
