using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes
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
