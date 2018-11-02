using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes
{
    public class LocalVariableScope : NestedScope
    {
        [NotNull] public new ExpressionSyntax Syntax { get; }

        public LocalVariableScope(
            [NotNull] LexicalScope containingScope,
            [NotNull] ExpressionSyntax syntax)
            : base(containingScope, syntax)
        {
            Syntax = syntax;
        }
    }
}
