using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class ExpressionStatementSyntax : StatementSyntax, IExpressionStatementSyntax
    {
        private IExpressionSyntax expression;
        public ref IExpressionSyntax Expression
        {
            [DebuggerStepThrough]
            get => ref expression;
        }

        public ExpressionStatementSyntax(TextSpan span, IExpressionSyntax expression)
            : base(span)
        {
            this.expression = expression;
        }

        public override string ToString()
        {
            return Expression+";";
        }
    }
}
