using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class LoopExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public BlockSyntax Block { get; }

        public LoopExpressionSyntax(TextSpan span, [NotNull] BlockSyntax block)
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
