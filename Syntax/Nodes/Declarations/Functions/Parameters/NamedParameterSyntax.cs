using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens.Identifiers;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Parameters
{
    public class NamedParameterSyntax : ParameterSyntax
    {
        [CanBeNull] public ParamsKeywordToken ParamsKeyword { get; }
        [CanBeNull] public VarKeywordToken VarKeyword { get; }
        [NotNull] public IIdentifierToken Name { get; }
        [NotNull] public IColonToken Colon { get; }
        [NotNull] public ExpressionSyntax TypeExpression { get; }
        [CanBeNull] public EqualsToken Equals { get; }
        [CanBeNull] public ExpressionSyntax DefaultValue { get; }

        public NamedParameterSyntax(
            [CanBeNull] ParamsKeywordToken paramsKeyword,
            [CanBeNull] VarKeywordToken varKeyword,
            [NotNull] IIdentifierToken name,
            [NotNull] IColonToken colon,
            [NotNull] ExpressionSyntax typeExpression,
            [CanBeNull] EqualsToken equals,
            [CanBeNull] ExpressionSyntax defaultValue)
            : base(TextSpan.Covering(paramsKeyword?.Span, varKeyword?.Span, name.Span, typeExpression.Span))
        {
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(colon), colon);
            Requires.NotNull(nameof(typeExpression), typeExpression);
            ParamsKeyword = paramsKeyword;
            VarKeyword = varKeyword;
            Name = name;
            Colon = colon;
            TypeExpression = typeExpression;
            Equals = @equals;
            DefaultValue = defaultValue;
        }
    }
}
