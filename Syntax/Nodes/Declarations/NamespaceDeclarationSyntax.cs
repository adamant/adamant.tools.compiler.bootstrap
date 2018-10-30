using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class NamespaceDeclarationSyntax : DeclarationSyntax, INamespaceSyntax
    {
        [NotNull] SyntaxNode INamespaceSyntax.AsSyntaxNode => this;
    }
}
