using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class WhileExpressionSyntax : ExpressionSyntax, IWhileExpressionSyntax
    {
        private IExpressionSyntax condition;
        public ref IExpressionSyntax Condition => ref condition;

        public BlockSyntax Block { get; }

        public WhileExpressionSyntax(
            TextSpan span,
            IExpressionSyntax condition,
            BlockSyntax block)
            : base(span)
        {
            this.condition = condition;
            Block = block;
        }

        public override string ToString()
        {
            return $"while {condition} {Block}";
        }
    }
}
