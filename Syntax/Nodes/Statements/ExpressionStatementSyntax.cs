using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements
{
    public class ExpressionStatementSyntax : StatementSyntax
    {
        [NotNull]
        public ExpressionSyntax Expression { get; }

        [CanBeNull]
        public SemicolonToken Semicolon { get; }

        public ExpressionStatementSyntax(
            [NotNull] ExpressionSyntax expression,
            [CanBeNull] in SemicolonToken semicolon)
        {
            Requires.NotNull(nameof(expression), expression);
            Expression = expression;
            Semicolon = semicolon;
        }
    }
}
