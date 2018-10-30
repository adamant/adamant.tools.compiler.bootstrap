using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Contracts
{
    public class EnsuresSyntax : ContractSyntax
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
