using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class BlockSyntax : ExpressionBlockSyntax
    {
        [NotNull] public IOpenBraceTokenPlace OpenBrace { get; }
        [NotNull] [ItemNotNull] public FixedList<StatementSyntax> Statements { get; }
        [NotNull] public ICloseBraceTokenPlace CloseBrace { get; }

        public BlockSyntax(
            [NotNull] IOpenBraceTokenPlace openBrace,
            [NotNull][ItemNotNull] FixedList<StatementSyntax> statements,
            [NotNull] ICloseBraceTokenPlace closeBrace)
            : base(TextSpan.Covering(openBrace.Span, closeBrace.Span))
        {
            OpenBrace = openBrace;
            Statements = statements;
            CloseBrace = closeBrace;
        }
    }
}
