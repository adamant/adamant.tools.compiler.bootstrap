using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class LoopExpression : Expression, ILoopExpression
    {
        public IBlockExpression Block { get; }

        public LoopExpression(
            TextSpan span,
            DataType dataType,
            ExpressionSemantics semantics,
            IBlockExpression block)
            : base(span, dataType, semantics)
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
