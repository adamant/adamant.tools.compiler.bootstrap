using Adamant.Tools.Compiler.Bootstrap.AST;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Scopes
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
