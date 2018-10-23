using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class NamespaceDeclarationSyntax : DeclarationSyntax, INamespaceSyntax
    {
        public override IIdentifierToken Name => throw new System.NotImplementedException();

        [NotNull] SyntaxNode INamespaceSyntax.AsSyntaxNode => this;
    }
}
