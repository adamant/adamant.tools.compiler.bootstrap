using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions
{
    public class BlockExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public IOpenBraceToken OpenBrace { get; }
        [NotNull] [ItemNotNull] public SyntaxList<StatementSyntax> Statements { get; }
        [NotNull] public ICloseBraceToken CloseBrace { get; }

        public BlockExpressionSyntax(
            [NotNull] IOpenBraceToken openBrace,
            [NotNull][ItemNotNull] SyntaxList<StatementSyntax> statements,
            [NotNull] ICloseBraceToken closeBrace)
            : base(TextSpan.Covering(openBrace.Span, closeBrace.Span))
        {
            Requires.NotNull(nameof(openBrace), openBrace);
            Requires.NotNull(nameof(statements), statements);
            Requires.NotNull(nameof(closeBrace), closeBrace);
            OpenBrace = openBrace;
            Statements = statements;
            CloseBrace = closeBrace;
        }
    }
}
