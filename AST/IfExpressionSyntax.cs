using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class IfExpressionSyntax : ExpressionSyntax, IElseClauseSyntax
    {
        public ExpressionSyntax Condition;
        public IBlockOrResultSyntax ThenBlock { get; }
        public IElseClauseSyntax ElseClause;

        public IfExpressionSyntax(
            TextSpan span,
            ExpressionSyntax condition,
            IBlockOrResultSyntax thenBlock,
            IElseClauseSyntax elseClause)
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
