using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow
{
    public class ReturnExpressionSyntax : ExpressionSyntax
    {
        [CanBeNull]
        public ReturnKeywordToken ReturnKeyword { get; }

        [NotNull]
        public ExpressionSyntax Expression { get; }

        public ReturnExpressionSyntax(
            [CanBeNull] ReturnKeywordToken returnKeyword,
            [NotNull] ExpressionSyntax expression)
        {
            Requires.NotNull(nameof(expression), expression);
            ReturnKeyword = returnKeyword;
            Expression = expression;
        }
    }
}
