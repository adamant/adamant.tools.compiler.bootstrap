using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class ExpressionStatementSyntax : StatementSyntax, IExpressionStatementSyntax
    {
        private ExpressionSyntax expression;
        public ExpressionSyntax Expression => expression;
        public ref ExpressionSyntax ExpressionRef => ref expression;

        public ExpressionStatementSyntax(TextSpan span, ExpressionSyntax expression)
            : base(span)
        {
            this.expression = expression;
        }

        public override string ToString()
        {
            return Expression.ToString();
        }
    }
}
