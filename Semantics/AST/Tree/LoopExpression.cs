using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class LoopExpression : Expression, ILoopExpression
    {
        public IBlockExpression Block { get; }

        public LoopExpression(TextSpan span, DataType dataType, IBlockExpression block)
            : base(span, dataType)
        {
            Block = block;
        }
    }
}
