using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow
{
    public class ReturnExpressionSyntax : ExpressionSyntax
    {
        [CanBeNull]
        public ReturnKeywordToken ReturnKeyword { get; }

        [CanBeNull]
        public ExpressionSyntax Expression { get; }

        public ReturnExpressionSyntax(
            [CanBeNull] ReturnKeywordToken returnKeyword,
            [CanBeNull] ExpressionSyntax expression)
        {
            ReturnKeyword = returnKeyword;
            Expression = expression;
        }
    }
}
