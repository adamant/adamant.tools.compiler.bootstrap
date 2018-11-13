using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class NamedParameterSyntax : ParameterSyntax
    {
        [CanBeNull] public IParamsKeywordToken ParamsKeyword { get; }
        [CanBeNull] public IVarKeywordToken VarKeyword { get; }
        [NotNull] public IIdentifierTokenPlace Name { get; }
        [NotNull] public IColonTokenPlace Colon { get; }
        [NotNull] public ExpressionSyntax TypeExpression { get; }
        [CanBeNull] public IEqualsToken EqualsToken { get; }
        [CanBeNull] public ExpressionSyntax DefaultValue { get; }

        public NamedParameterSyntax(
            [CanBeNull] IParamsKeywordToken paramsKeyword,
            [CanBeNull] IVarKeywordToken varKeyword,
            [NotNull] IIdentifierTokenPlace name,
            [NotNull] IColonTokenPlace colon,
            [NotNull] ExpressionSyntax typeExpression,
            [CanBeNull] IEqualsToken equalsToken,
            [CanBeNull] ExpressionSyntax defaultValue)
            : base(TextSpan.Covering(paramsKeyword?.Span, varKeyword?.Span, name?.Span, typeExpression.Span))
        {
            ParamsKeyword = paramsKeyword;
            VarKeyword = varKeyword;
            Name = name;
            Colon = colon;
            TypeExpression = typeExpression;
            EqualsToken = equalsToken;
            DefaultValue = defaultValue;
        }
    }
}
