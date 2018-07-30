using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions.ControlFlow
{
    public class ReturnExpression : Expression
    {
        public new ReturnExpressionSyntax Syntax { get; }
        public Expression Expression { get; }

        public ReturnExpression(ReturnExpressionSyntax syntax, Expression expression)
            : base(PrimitiveType.Never)
        {
            Syntax = syntax;
            Expression = expression;
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }
    }
}
