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
        [NotNull] public FixedList<MatchArmSyntax> Arms { get; }
        [NotNull] public ICloseBraceTokenPlace CloseBrace { get; }

        public MatchExpressionSyntax(
            [NotNull] IMatchKeywordToken matchKeyword,
            [NotNull] ExpressionSyntax value,
            [NotNull] IOpenBraceTokenPlace openBrace,
            [NotNull] FixedList<MatchArmSyntax> arms,
            [NotNull] ICloseBraceTokenPlace closeBrace)
            : base(TextSpan.Covering(matchKeyword.Span, closeBrace.Span))
        {
            MatchKeyword = matchKeyword;
            Value = value;
            OpenBrace = openBrace;
            Arms = arms;
            CloseBrace = closeBrace;
        }
    }
}
