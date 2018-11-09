using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ExpressionStatementSyntax : StatementSyntax
    {
        [NotNull] public ExpressionSyntax Expression { get; }
        // Block statements are expression statements without a semicolon
        [CanBeNull] public ISemicolonToken Semicolon { get; }

        public ExpressionStatementSyntax(
            [NotNull] ExpressionSyntax expression,
            [CanBeNull] ISemicolonToken semicolon)
        {
            Requires.NotNull(nameof(expression), expression);
            Expression = expression;
            Semicolon = semicolon;
        }
    }
}
