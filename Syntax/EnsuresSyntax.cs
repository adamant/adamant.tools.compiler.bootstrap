using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class EnsuresSyntax : FunctionContractSyntax
    {
        [NotNull] public EnsuresKeywordToken EnsuresKeyword { get; }
        [NotNull] public ExpressionSyntax Condition { get; }

        public EnsuresSyntax(
            [NotNull] EnsuresKeywordToken ensuresKeyword,
            [NotNull] ExpressionSyntax condition)
        {
            Requires.NotNull(nameof(ensuresKeyword), ensuresKeyword);
            Requires.NotNull(nameof(condition), condition);
            EnsuresKeyword = ensuresKeyword;
            Condition = condition;
        }
    }
}
