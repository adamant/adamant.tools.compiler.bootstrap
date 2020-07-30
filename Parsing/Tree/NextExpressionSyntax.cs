using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class NextExpressionSyntax : ExpressionSyntax, INextExpressionSyntax
    {
        public NextExpressionSyntax(TextSpan span)
            : base(span, ExpressionSemantics.Never)
        {
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return "next";
        }
    }
}
