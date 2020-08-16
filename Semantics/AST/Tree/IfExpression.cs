using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class IfExpression : Expression, IIfExpression
    {
        public IExpression Condition { get; }
        public IBlockOrResult ThenBlock { get; }
        public IElseClause? ElseClause { get; }

        public IfExpression(
            TextSpan span,
            DataType dataType,
            ExpressionSemantics semantics,
            IExpression condition,
            IBlockOrResult thenBlock,
            IElseClause? elseClause)
            : base(span, dataType, semantics)
        {
            Condition = condition;
            ThenBlock = thenBlock;
            ElseClause = elseClause;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

        public override string ToString()
        {
            if (ElseClause != null) return $"if {Condition} {ThenBlock} else {ElseClause}";
            return $"if {Condition} {ThenBlock}";
        }
    }
}
