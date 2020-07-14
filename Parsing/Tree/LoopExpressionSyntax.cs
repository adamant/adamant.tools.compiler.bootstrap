using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.FST;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class LoopExpressionSyntax : ExpressionSyntax, ILoopExpressionSyntax
    {
        public IBlockExpressionSyntax Block { get; }

        public LoopExpressionSyntax(TextSpan span, IBlockExpressionSyntax block)
            : base(span)
        {
            Block = block;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return $"loop {Block}";
        }
    }
}
