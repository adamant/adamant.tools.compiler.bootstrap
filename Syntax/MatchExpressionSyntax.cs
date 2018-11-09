using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class MatchExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public IMatchKeywordToken MatchKeyword { get; }
        [NotNull] public ExpressionSyntax Value { get; }
        [NotNull] public IOpenBraceTokenPlace OpenBrace { get; }
        [NotNull] public SyntaxList<MatchArmSyntax> Arms { get; }
        [NotNull] public ICloseBraceTokenPlace CloseBrace { get; }

        public MatchExpressionSyntax(
            [NotNull] IMatchKeywordToken matchKeyword,
            [NotNull] ExpressionSyntax value,
            [NotNull] IOpenBraceTokenPlace openBrace,
            [NotNull] SyntaxList<MatchArmSyntax> arms,
            [NotNull] ICloseBraceTokenPlace closeBrace)
            : base(TextSpan.Covering(matchKeyword.Span, closeBrace.Span))
        {
            Requires.NotNull(nameof(matchKeyword), matchKeyword);
            Requires.NotNull(nameof(value), value);
            Requires.NotNull(nameof(openBrace), openBrace);
            Requires.NotNull(nameof(arms), arms);
            Requires.NotNull(nameof(closeBrace), closeBrace);
            MatchKeyword = matchKeyword;
            Value = value;
            OpenBrace = openBrace;
            Arms = arms;
            CloseBrace = closeBrace;
        }
    }
}
