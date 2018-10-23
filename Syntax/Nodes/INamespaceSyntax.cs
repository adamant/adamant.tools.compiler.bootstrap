using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public interface INamespaceSyntax
    {
        [NotNull] SyntaxNode AsSyntaxNode { get; }
    }
}
