using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements
{
    public class ExpressionStatementSyntax : StatementSyntax
    {
        [NotNull] public ExpressionSyntax Expression { get; }
        [NotNull] public ISemicolonToken Semicolon { get; }

        public ExpressionStatementSyntax(
            [NotNull] ExpressionSyntax expression,
            [NotNull] ISemicolonToken semicolon)
        {
            Requires.NotNull(nameof(expression), expression);
            Expression = expression;
            Semicolon = semicolon;
        }
    }
}
