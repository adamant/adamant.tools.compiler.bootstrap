using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class BaseTypesSyntax : SyntaxNode
    {
        [NotNull] public LessThanColonToken LessThanColon { get; }
        [NotNull] public SeparatedListSyntax<ExpressionSyntax> TypeExpressions { get; }

        public BaseTypesSyntax(
            [NotNull] LessThanColonToken lessThanColon,
            [NotNull] SeparatedListSyntax<ExpressionSyntax> typeExpressions)
        {
            Requires.NotNull(nameof(lessThanColon), lessThanColon);
            Requires.NotNull(nameof(typeExpressions), typeExpressions);
            LessThanColon = lessThanColon;
            TypeExpressions = typeExpressions;
        }
    }
}
