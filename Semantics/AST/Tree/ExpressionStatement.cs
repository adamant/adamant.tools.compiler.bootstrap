using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class ExpressionStatement : Statement, IExpressionStatement
    {
        public IExpression Expression { [DebuggerStepThrough] get; }

        public ExpressionStatement(TextSpan span, IExpression expression)
            : base(span)
        {
            Expression = expression;
        }

        public override string ToString()
        {
            return Expression + ";";
        }
    }
}
