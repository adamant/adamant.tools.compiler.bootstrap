using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
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
            IExpression condition,
            IBlockOrResult thenBlock,
            IElseClause? elseClause)
            : base(span, dataType)
        {
            Condition = condition;
            ThenBlock = thenBlock;
            ElseClause = elseClause;
        }
    }
}
