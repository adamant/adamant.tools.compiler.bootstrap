using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public abstract class DeclarationSyntax : SyntaxNode
    {
        public abstract IdentifierToken Name { get; }
    }
}
