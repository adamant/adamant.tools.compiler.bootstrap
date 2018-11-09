using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class MatchArmSyntax : NonTerminal
    {
        [NotNull] public PatternSyntax Pattern { get; }
        [NotNull] public ExpressionBlockSyntax Expression { get; }
        [CanBeNull] public ICommaToken Comma { get; }

        public MatchArmSyntax(
            [NotNull] PatternSyntax pattern,
            [NotNull] ExpressionBlockSyntax expression,
            [CanBeNull] ICommaToken comma)
        {
            Requires.NotNull(nameof(pattern), pattern);
            Requires.NotNull(nameof(expression), expression);
            Pattern = pattern;
            Expression = expression;
            Comma = comma;
        }
    }
}
