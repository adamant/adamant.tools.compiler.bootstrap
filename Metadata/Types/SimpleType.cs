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

        public override ValueSemantics ValueSemantics => ValueSemantics.Copy;

        public override OldValueSemantics OldValueSemantics => OldValueSemantics.Copy;

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
