using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

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

        public override string ToString()
        {
            return $"loop {Block}";
        }
    }
}