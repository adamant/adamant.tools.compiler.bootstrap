using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    /// <summary>
    /// A result statement must be the last statement of the enclosing block
    /// </summary>
    internal class ResultStatementSyntax : StatementSyntax, IResultStatementSyntax
    {
        private IExpressionSyntax expression;

        public IExpressionSyntax Expression => expression;

        public ref IExpressionSyntax ExpressionRef => ref expression;

        public ResultStatementSyntax(
            TextSpan span,
            IExpressionSyntax expression)
            : base(span)
        {
            this.expression = expression;
        }

        public override string ToString()
        {
            return $"=> {Expression}";
        }
    }
}
