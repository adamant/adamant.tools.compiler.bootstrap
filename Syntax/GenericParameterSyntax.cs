using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class GenericParameterSyntax : NonTerminal
    {
        [CanBeNull] public ParamsKeywordToken ParamsKeyword { get; }
        [NotNull] public IIdentifierToken Name { get; }
        [CanBeNull] public IColonToken Colon { get; }
        [CanBeNull] public ExpressionSyntax TypeExpression { get; }

        public GenericParameterSyntax(
            [CanBeNull] ParamsKeywordToken paramsKeyword,
            [NotNull] IIdentifierToken name,
            [CanBeNull] IColonToken colon,
            [CanBeNull] ExpressionSyntax typeExpression)
        {
            Requires.NotNull(nameof(name), name);
            ParamsKeyword = paramsKeyword;
            Name = name;
            Colon = colon;
            TypeExpression = typeExpression;
        }
    }
}
