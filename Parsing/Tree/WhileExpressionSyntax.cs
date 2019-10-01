using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class WhileExpressionSyntax : ExpressionSyntax, IWhileExpressionSyntax
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
