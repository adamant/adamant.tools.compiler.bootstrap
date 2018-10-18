using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow
{
    public class ReturnExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public IReturnKeywordToken ReturnKeyword { get; }
        [CanBeNull] public ExpressionSyntax Expression { get; }

        public ReturnExpressionSyntax(
            [NotNull] IReturnKeywordToken returnKeyword,
            [CanBeNull] ExpressionSyntax expression)
        {
            Requires.NotNull(nameof(returnKeyword), returnKeyword);
            ReturnKeyword = returnKeyword;
            Expression = expression;
        }
    }
}
