using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    [Closed(
        typeof(NamespaceSymbol),
        typeof(PackageSymbol))]
    public abstract class NamespaceOrPackageSymbol : Symbol
    {
        public new Name Name { get; }

        protected NamespaceOrPackageSymbol(NamespaceOrPackageSymbol? containingSymbol, Name name)
            : base(containingSymbol, name)
        {
            Name = name;
        }
    }
}
