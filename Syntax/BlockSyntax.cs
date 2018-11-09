using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class BlockSyntax : ExpressionBlockSyntax
    {
        [NotNull] public IOpenBraceTokenPlace OpenBrace { get; }
        [NotNull] [ItemNotNull] public SyntaxList<StatementSyntax> Statements { get; }
        [NotNull] public ICloseBraceTokenPlace CloseBrace { get; }

        public BlockSyntax(
            [NotNull] IOpenBraceTokenPlace openBrace,
            [NotNull][ItemNotNull] SyntaxList<StatementSyntax> statements,
            [NotNull] ICloseBraceTokenPlace closeBrace)
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
