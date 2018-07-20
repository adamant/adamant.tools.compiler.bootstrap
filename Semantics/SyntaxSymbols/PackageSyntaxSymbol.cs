using Adamant.Tools.Compiler.Bootstrap.Syntax;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols
{
    public class PackageSyntaxSymbol
    {
        public string Name { get; }
        public readonly NamespaceSyntaxSymbol GlobalNamespace = new NamespaceSyntaxSymbol("");
        public readonly PackageSyntax Declaration;

        public PackageSyntaxSymbol(PackageSyntax package)
        {
            Name = "default"; // TODO we should get this from the package syntax?
            Declaration = package;
        }
    }
}
