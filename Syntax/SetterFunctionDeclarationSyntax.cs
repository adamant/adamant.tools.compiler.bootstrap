using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class SetterFunctionDeclarationSyntax : FunctionDeclarationSyntax
    {
        [NotNull] public ISetKeywordToken SetKeyword { get; }
        [NotNull] public override IIdentifierTokenPlace Name { get; }
        [NotNull] public ExpressionSyntax ReturnTypeExpression { get; }

        public SetterFunctionDeclarationSyntax(
            [NotNull] FixedList<IModiferToken> modifiers,
            [NotNull] ISetKeywordToken setKeyword,
            [NotNull] IIdentifierTokenPlace name,
            [NotNull] FixedList<ParameterSyntax> parameters,
            [NotNull] ExpressionSyntax returnTypeExpression,
            [NotNull] FixedList<EffectSyntax> mayEffects,
            [NotNull] FixedList<EffectSyntax> noEffects,
            [NotNull] FixedList<ExpressionSyntax> requires,
            [NotNull] FixedList<ExpressionSyntax> ensures,
            [CanBeNull] BlockSyntax body)
            : base(TextSpan.Covering(setKeyword.Span, name.Span), modifiers,
                parameters, mayEffects, noEffects, requires, ensures, body)
        {
            SetKeyword = setKeyword;
            Name = name;
            ReturnTypeExpression = returnTypeExpression;
        }
    }
}
