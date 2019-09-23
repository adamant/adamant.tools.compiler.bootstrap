using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// A result statement must be the last statement of the enclosing block
    /// </summary>
    public class ResultStatementSyntax : StatementSyntax, IBlockOrResultSyntax
    {
        public ExpressionSyntax Expression;

        public ResultStatementSyntax(
            TextSpan span,
            ExpressionSyntax expression)
            : base(span)
        {
            Expression = expression;
        }

        public override string ToString()
        {
            return $"=> {Expression}";
        }
    }
}
