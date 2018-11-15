using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    // TODO just make expressions into statements
    public class ExpressionStatementSyntax : StatementSyntax
    {
        [NotNull] public ExpressionSyntax Expression { get; }

        public ExpressionStatementSyntax([NotNull] ExpressionSyntax expression)
        {
            Expression = expression;
        }
    }
}
