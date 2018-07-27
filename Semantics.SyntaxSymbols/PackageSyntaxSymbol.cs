using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols
{
    internal class PackageSyntaxSymbol : SyntaxSymbol<PackageSyntax>, IPackageSyntaxSymbol
    {
        public NamespaceSyntaxSymbol GlobalNamespace { get; }
        INamespaceSyntaxSymbol IPackageSyntaxSymbol.GlobalNamespace => GlobalNamespace;

        public PackageSyntax Declaration { get; }

        public PackageSyntaxSymbol(PackageSyntax package)
            : base("default", package) // TODO we should get name from the package syntax
        {
            GlobalNamespace = new NamespaceSyntaxSymbol(package);
            Declaration = package;
            Add(GlobalNamespace);
        }
    }
}
