using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ReturnExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax ReturnValue;

        public ReturnExpressionSyntax(
            TextSpan span,
            ExpressionSyntax returnValue)
            : base(span)
        {
            ReturnValue = returnValue;
        }

        public override string ToString()
        {
            if (ReturnValue != null)
                return $"return {ReturnValue}";
            return "return";
        }
    }
}
