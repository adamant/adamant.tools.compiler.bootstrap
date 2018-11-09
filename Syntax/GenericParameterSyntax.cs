using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class GenericParameterSyntax : NonTerminal
    {
        [CanBeNull] public IParamsKeywordToken ParamsKeyword { get; }
        [NotNull] public IIdentifierTokenPlace Name { get; }
        [CanBeNull] public IColonTokenPlace Colon { get; }
        [CanBeNull] public ExpressionSyntax TypeExpression { get; }

        public GenericParameterSyntax(
            [CanBeNull] IParamsKeywordToken paramsKeyword,
            [NotNull] IIdentifierTokenPlace name,
            [CanBeNull] IColonTokenPlace colon,
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
