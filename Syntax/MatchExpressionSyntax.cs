using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class MatchExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public MatchKeywordToken MatchKeyword { get; }
        [NotNull] public ExpressionSyntax Value { get; }
        [NotNull] public IOpenBraceToken OpenBrace { get; }
        [NotNull] public SyntaxList<MatchArmSyntax> Arms { get; }
        [NotNull] public ICloseBraceToken CloseBrace { get; }

        public MatchExpressionSyntax(
            [NotNull] MatchKeywordToken matchKeyword,
            [NotNull] ExpressionSyntax value,
            [NotNull] IOpenBraceToken openBrace,
            [NotNull] SyntaxList<MatchArmSyntax> arms,
            [NotNull] ICloseBraceToken closeBrace)
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
