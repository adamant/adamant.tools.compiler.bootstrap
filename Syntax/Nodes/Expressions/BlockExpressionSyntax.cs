using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions
{
    public class BlockExpressionSyntax : ExpressionSyntax
    {
        [CanBeNull]
        public OpenBraceToken OpenBrace { get; }

        [NotNull]
        public SyntaxList<StatementSyntax> Statements { get; }

        [CanBeNull]
        public CloseBraceToken CloseBrace { get; }

        public BlockExpressionSyntax(
            [CanBeNull] OpenBraceToken openBrace,
            [NotNull] SyntaxList<StatementSyntax> statements,
            [CanBeNull] CloseBraceToken closeBrace)
        {
            Requires.NotNull(nameof(statements), statements);
            OpenBrace = openBrace;
            Statements = statements;
            CloseBrace = closeBrace;
        }
    }
}
