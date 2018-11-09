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
        [NotNull] public IRightArrowTokenPlace Arrow { get; }
        [NotNull] public ExpressionSyntax ReturnTypeExpression { get; }

        public SetterFunctionDeclarationSyntax(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ISetKeywordToken setKeyword,
            [NotNull] IIdentifierTokenPlace name,
            [NotNull] IOpenParenTokenPlace openParen,
            [NotNull] SeparatedListSyntax<ParameterSyntax> parameterList,
            [NotNull] ICloseParenTokenPlace closeParen,
            [NotNull] IRightArrowTokenPlace arrow,
            [NotNull] ExpressionSyntax returnTypeExpression,
            [CanBeNull] EffectsSyntax effects,
            [NotNull] SyntaxList<FunctionContractSyntax> contracts,
            [CanBeNull] BlockSyntax body,
            [CanBeNull] ISemicolonTokenPlace semicolon)
            : base(TextSpan.Covering(setKeyword.Span, name.Span), modifiers, openParen,
                parameterList, closeParen, effects, contracts, body, semicolon)
        {
            Requires.NotNull(nameof(modifiers), modifiers);
            Requires.NotNull(nameof(setKeyword), setKeyword);
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(openParen), openParen);
            Requires.NotNull(nameof(parameterList), parameterList);
            Requires.NotNull(nameof(closeParen), closeParen);
            Requires.NotNull(nameof(arrow), arrow);
            Requires.NotNull(nameof(returnTypeExpression), returnTypeExpression);
            SetKeyword = setKeyword;
            Name = name;
            Arrow = arrow;
            ReturnTypeExpression = returnTypeExpression;
        }
    }
}
