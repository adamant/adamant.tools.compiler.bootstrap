using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class IfExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Condition { get; }
        public ExpressionBlockSyntax ThenBlock { get; }
        public ExpressionSyntax ElseClause { get; }

        public IfExpressionSyntax(
            TextSpan span,
            ExpressionSyntax condition,
            ExpressionBlockSyntax thenBlock,
            ExpressionSyntax elseClause)
            : base(span)
        {
            Condition = condition;
            ThenBlock = thenBlock;
            ElseClause = elseClause;
        }

        public override string ToString()
        {
            if (ElseClause != null)
                return $"if {Condition} {ThenBlock} else {ElseClause}";
            return $"if {Condition} {ThenBlock}";
        }
    }
}
