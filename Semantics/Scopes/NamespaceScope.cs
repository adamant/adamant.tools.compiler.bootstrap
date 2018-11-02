using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes
{
    public class NamespaceScope : NestedScope
    {
        [NotNull] public new NamespaceDeclarationSyntax Syntax { get; }

        public NamespaceScope(
            [NotNull] LexicalScope containingScope,
            [NotNull] NamespaceDeclarationSyntax @namespace)
            : base(containingScope, @namespace)
        {
            Syntax = @namespace;
        }
    }
}
