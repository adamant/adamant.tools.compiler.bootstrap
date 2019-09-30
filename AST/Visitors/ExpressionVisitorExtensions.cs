using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Visitors
{
    public static class ExpressionVisitorExtensions
    {
        public static void VisitStatement(this ExpressionVisitor<Void> visitor, IStatementSyntax statement)
        {
            visitor.VisitStatement(statement, default);
        }

        public static void VisitExpression(this ExpressionVisitor<Void> visitor, ExpressionSyntax expression)
        {
            visitor.VisitExpression(expression, default);
        }
    }
}
