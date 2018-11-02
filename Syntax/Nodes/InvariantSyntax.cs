using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class InvariantSyntax : SyntaxNode
    {
        [NotNull] public InvariantKeywordToken InvariantKeyword { get; }
        [NotNull] public ExpressionSyntax Condition { get; }

        public InvariantSyntax(
            [NotNull] InvariantKeywordToken invariantKeyword,
            [NotNull] ExpressionSyntax condition)
        {
            Requires.NotNull(nameof(invariantKeyword), invariantKeyword);
            Requires.NotNull(nameof(condition), condition);
            InvariantKeyword = invariantKeyword;
            Condition = condition;
        }
    }
}
