using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class ExpressionStatement : Statement, IExpressionStatement
    {
        public IExpression Expression { get; }

        public ExpressionStatement(TextSpan span, IExpression expression)
            : base(span)
        {
            Expression = expression;
        }
    }
}
