using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes
{
    public class UsingDirectivesScope : NestedScope
    {
        [NotNull] public new NamespaceDeclarationSyntax Syntax { get; }

        public UsingDirectivesScope(
            [NotNull] LexicalScope containingScope,
            [NotNull] NamespaceDeclarationSyntax @namespace)
            : base(containingScope, @namespace)
        {
            Syntax = @namespace;
        }
    }
}
