using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class GenericConstraintSyntax : NonTerminal
    {
        [NotNull] public WhereKeywordToken WhereKeyword { get; }
        [NotNull] public ExpressionSyntax Expression { get; }

        public GenericConstraintSyntax(
            [NotNull] WhereKeywordToken whereKeyword,
            [NotNull] ExpressionSyntax expression)
        {
            Requires.NotNull(nameof(whereKeyword), whereKeyword);
            Requires.NotNull(nameof(expression), expression);
            WhereKeyword = whereKeyword;
            Expression = expression;
        }
    }
}
