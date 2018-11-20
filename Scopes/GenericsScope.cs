using Adamant.Tools.Compiler.Bootstrap.AST;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Scopes
{
    public class GenericsScope : NestedScope
    {
        [NotNull] public new MemberDeclarationSyntax Syntax { get; }

        public GenericsScope(
            [NotNull] LexicalScope containingScope,
            [NotNull] MemberDeclarationSyntax syntax)
            : base(containingScope, syntax)
        {
            Syntax = syntax;
        }
    }
}
