using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ExpressionStatementSyntax : StatementSyntax
    {
        public ExpressionSyntax Expression;

        public ExpressionStatementSyntax(TextSpan span, ExpressionSyntax expression)
            : base(span)
        {
            Expression = expression;
        }

        public override string ToString()
        {
            return Expression.ToString();
        }
    }
}
