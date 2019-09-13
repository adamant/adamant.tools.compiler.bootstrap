using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    // TODO should this really be a TypeSyntax? Maybe it should be just ExpressionSyntax
    public sealed class MutableExpressionSyntax : TypeSyntax
    {
        public ExpressionSyntax Expression { get; }

        public MutableExpressionSyntax(TextSpan span, ExpressionSyntax expression)
            : base(span)
        {
            Expression = expression;
        }

        public override string ToString()
        {
            return $"mut {Expression}";
        }
    }
}
