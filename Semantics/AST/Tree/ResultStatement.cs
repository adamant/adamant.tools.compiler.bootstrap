using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class ResultStatement : Statement, IResultStatement
    {
        public IExpression Expression { get; }

        public ResultStatement(TextSpan span, IExpression expression)
            : base(span)
        {
            Expression = expression;
        }

        public override string ToString()
        {
            return $"=> {Expression};";
        }
    }
}
