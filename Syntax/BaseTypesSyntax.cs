using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class BaseTypesSyntax : NonTerminal
    {
        [NotNull] public ILessThanColonToken LessThanColon { get; }
        [NotNull] public SeparatedListSyntax<ExpressionSyntax> TypeExpressions { get; }

        public BaseTypesSyntax(
            [NotNull] ILessThanColonToken lessThanColon,
            [NotNull] SeparatedListSyntax<ExpressionSyntax> typeExpressions)
        {
            Requires.NotNull(nameof(lessThanColon), lessThanColon);
            Requires.NotNull(nameof(typeExpressions), typeExpressions);
            LessThanColon = lessThanColon;
            TypeExpressions = typeExpressions;
        }
    }
}
