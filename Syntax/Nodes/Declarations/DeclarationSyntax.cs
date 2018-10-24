using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    // TODO remove this class, and just make everything a member declaration the same as Roslyn does
    public abstract class DeclarationSyntax : SyntaxNode
    {
        [NotNull] public abstract IIdentifierToken Name { get; }
    }
}
