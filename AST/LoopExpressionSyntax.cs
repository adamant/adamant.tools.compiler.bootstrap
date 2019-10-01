using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class LoopExpressionSyntax : ExpressionSyntax, ILoopExpressionSyntax
    {
        public BlockSyntax Block { get; }

        public LoopExpressionSyntax(TextSpan span, BlockSyntax block)
            : base(span)
        {
            Block = block;
        }

        public override string ToString()
        {
            return $"loop {Block}";
        }
    }
}
