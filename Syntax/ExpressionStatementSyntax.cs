using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ExpressionStatementSyntax : StatementSyntax
    {
        [NotNull] public ExpressionSyntax Expression { get; }
        // Block statements are expression statements without a semicolon
        [CanBeNull] public ISemicolonTokenPlace Semicolon { get; }

        public ExpressionStatementSyntax(
            [NotNull] ExpressionSyntax expression,
            [CanBeNull] ISemicolonTokenPlace semicolon)
        {
            Requires.NotNull(nameof(expression), expression);
            Expression = expression;
            Semicolon = semicolon;
        }
    }
}
