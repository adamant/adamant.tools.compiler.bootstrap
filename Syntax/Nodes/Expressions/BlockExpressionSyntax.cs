using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions
{
    public class BlockExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public IOpenBraceToken OpenBrace { get; }
        [NotNull] public SyntaxList<StatementSyntax> Statements { get; }
        [NotNull] public ICloseBraceToken CloseBrace { get; }

        public BlockExpressionSyntax(
            [NotNull] IOpenBraceToken openBrace,
            [NotNull] SyntaxList<StatementSyntax> statements,
            [NotNull] ICloseBraceToken closeBrace)
        {
            Requires.NotNull(nameof(statements), statements);
            OpenBrace = openBrace;
            Statements = statements;
            CloseBrace = closeBrace;
        }
    }
}
