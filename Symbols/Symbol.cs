using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    [Closed(
        typeof(ParentSymbol),
        typeof(BindingSymbol))]
    public abstract class Symbol
    {
        public Name FullName { get; }

        public bool IsGlobal => FullName is SimpleName;

        protected Symbol(Name fullName)
        {
            // Note: constructor can't be `private protected` so `Symbol` can be mocked in unit tests
            FullName = fullName;
        }
    }
}
