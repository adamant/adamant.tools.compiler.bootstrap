using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class WhileExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Condition;
        public BlockSyntax Block { get; }

        public WhileExpressionSyntax(
            TextSpan span,
            ExpressionSyntax condition,
            BlockSyntax block)
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
