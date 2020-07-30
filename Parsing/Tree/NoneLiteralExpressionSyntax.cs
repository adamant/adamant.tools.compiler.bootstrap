using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class NoneLiteralExpressionSyntax : LiteralExpressionSyntax, INoneLiteralExpressionSyntax
    {
        public NoneLiteralExpressionSyntax(TextSpan span)
            : base(span, ExpressionSemantics.Copy)
        {
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return "none";
        }
    }
}
