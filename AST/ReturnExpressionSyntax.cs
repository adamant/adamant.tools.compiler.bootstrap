using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ReturnExpressionSyntax : ExpressionSyntax
    {
        [CanBeNull] public ExpressionSyntax ReturnValue { get; }

        public ReturnExpressionSyntax(
            TextSpan span,
            [CanBeNull] ExpressionSyntax returnValue)
            : base(span)
        {
            ReturnValue = returnValue;
        }

        public override string ToString()
        {
            if (ReturnValue != null) return $"return {ReturnValue}";
            return "return";
        }
    }
}
