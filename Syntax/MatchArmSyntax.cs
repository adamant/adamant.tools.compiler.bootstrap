using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class MatchArmSyntax : NonTerminal
    {
        [NotNull] public PatternSyntax Pattern { get; }
        [NotNull] public ExpressionBlockSyntax Expression { get; }

        public MatchArmSyntax(
            [NotNull] PatternSyntax pattern,
            [NotNull] ExpressionBlockSyntax expression)
        {
            Pattern = pattern;
            Expression = expression;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
