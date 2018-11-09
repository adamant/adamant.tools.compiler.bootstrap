using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class InvariantSyntax : NonTerminal
    {
        [NotNull] public IInvariantKeywordToken InvariantKeyword { get; }
        [NotNull] public ExpressionSyntax Condition { get; }

        public InvariantSyntax(
            [NotNull] IInvariantKeywordToken invariantKeyword,
            [NotNull] ExpressionSyntax condition)
        {
            Requires.NotNull(nameof(invariantKeyword), invariantKeyword);
            Requires.NotNull(nameof(condition), condition);
            InvariantKeyword = invariantKeyword;
            Condition = condition;
        }
    }
}
