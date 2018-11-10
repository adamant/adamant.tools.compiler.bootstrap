using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class GetterFunctionDeclarationSyntax : FunctionDeclarationSyntax
    {
        [NotNull] public IGetKeywordToken GetKeyword { get; }
        [NotNull] public override IIdentifierTokenPlace Name { get; }
        [NotNull] public IRightArrowTokenPlace Arrow { get; }
        [NotNull] public ExpressionSyntax ReturnTypeExpression { get; }

        public GetterFunctionDeclarationSyntax(
            [NotNull] FixedList<IModiferToken> modifiers,
            [NotNull] IGetKeywordToken getKeyword,
            [NotNull] IIdentifierTokenPlace name,
            [NotNull] FixedList<ParameterSyntax> parameters,
            [NotNull] IRightArrowTokenPlace arrow,
            [NotNull] ExpressionSyntax returnTypeExpression,
            [NotNull] FixedList<EffectSyntax> mayEffects,
            [NotNull] FixedList<EffectSyntax> noEffects,
            [NotNull] FixedList<ExpressionSyntax> requires,
            [NotNull] FixedList<ExpressionSyntax> ensures,
            [CanBeNull] BlockSyntax body)
            : base(TextSpan.Covering(getKeyword.Span, name.Span),
            modifiers, parameters, mayEffects, noEffects, requires, ensures, body)
        {
            GetKeyword = getKeyword;
            Name = name;
            Arrow = arrow;
            ReturnTypeExpression = returnTypeExpression;
        }
    }
}
