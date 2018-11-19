using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class WhileExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public ExpressionSyntax Condition { get; }
        [NotNull] public BlockSyntax Block { get; }

        public WhileExpressionSyntax(
            TextSpan span,
            [NotNull] ExpressionSyntax condition,
            [NotNull] BlockSyntax block)
            : base(span)
        {
            Condition = condition;
            Block = block;
        }

        public override string ToString()
        {
            return $"while {Condition} {Block}";
        }
    }
}
