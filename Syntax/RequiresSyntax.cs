using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class RequiresSyntax : FunctionContractSyntax
    {
        [NotNull] public RequiresKeywordToken RequiresKeyword { get; }
        [NotNull] public ExpressionSyntax Condition { get; }

        public RequiresSyntax(
            [NotNull] RequiresKeywordToken requiresKeyword,
            [NotNull] ExpressionSyntax condition)
        {
            Requires.NotNull(nameof(requiresKeyword), requiresKeyword);
            Requires.NotNull(nameof(condition), condition);
            RequiresKeyword = requiresKeyword;
            Condition = condition;
        }
    }
}
