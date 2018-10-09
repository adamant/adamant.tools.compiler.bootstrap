using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public abstract class DeclarationSyntax : SyntaxNode
    {
        [CanBeNull]
        public abstract IdentifierToken Name { get; }
    }
}
