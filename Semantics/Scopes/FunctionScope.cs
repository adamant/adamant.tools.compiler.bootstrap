using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes
{
    public class FunctionScope : NestedScope
    {
        [NotNull] public new FunctionDeclarationSyntax Syntax { get; }

        public FunctionScope(
            [NotNull] LexicalScope containingScope,
            [NotNull] FunctionDeclarationSyntax syntax)
            : base(containingScope, syntax)
        {
            Syntax = syntax;
        }
    }
}
