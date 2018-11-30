using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Visitors
{
    public static class ExpressionVisitorExtensions
    {
        public static void VisitStatement([NotNull] this ExpressionVisitor<Void> visitor, [CanBeNull] StatementSyntax statement)
        {
            visitor.VisitStatement(statement, default);
        }

        public static void VisitExpression([NotNull] this ExpressionVisitor<Void> visitor, [CanBeNull] ExpressionSyntax expression)
        {
            visitor.VisitStatement(expression, default);
        }
    }
}
