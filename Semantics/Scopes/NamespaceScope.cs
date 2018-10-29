using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes
{
    public class NamespaceScope : NestedScope
    {
        [NotNull] public new INamespaceSyntax Syntax { get; }

        public NamespaceScope(
            [NotNull] LexicalScope containingScope,
            [NotNull] INamespaceSyntax @namespace)
            : base(containingScope, @namespace.AsSyntaxNode)
        {
            Syntax = @namespace;
        }
    }
}
