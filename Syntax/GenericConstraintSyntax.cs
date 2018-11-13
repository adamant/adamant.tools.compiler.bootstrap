using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class GenericConstraintSyntax : NonTerminal
    {
        [NotNull] public ExpressionSyntax Expression { get; }

        public GenericConstraintSyntax(
            [NotNull] ExpressionSyntax expression)
        {
            Expression = expression;
        }
    }
}
