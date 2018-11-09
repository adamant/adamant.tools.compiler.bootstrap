using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public abstract class OperatorExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public IOperatorToken Operator { get; }

        protected OperatorExpressionSyntax([NotNull] IOperatorToken @operator, TextSpan span)
            : base(span)
        {
            Requires.NotNull(nameof(@operator), @operator);
            Operator = @operator;
        }
    }
}
