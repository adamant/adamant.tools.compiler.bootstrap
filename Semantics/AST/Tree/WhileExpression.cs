using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class WhileExpression : Expression, IWhileExpression
    {
        public IExpression Condition { get; }
        public IBlockExpression Block { get; }

        public WhileExpression(
            TextSpan span,
            DataType dataType,
            ExpressionSemantics semantics,
            IExpression condition,
            IBlockExpression block)
            : base(span, dataType, semantics)
        {
            Condition = condition;
            Block = block;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

        public override string ToString()
        {
            return $"while {Condition} {Block}";
        }
    }
}
