using Adamant.Tools.Compiler.Bootstrap.AST;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Scopes
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
