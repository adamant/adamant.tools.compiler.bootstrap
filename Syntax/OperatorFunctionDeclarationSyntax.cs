using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class OperatorFunctionDeclarationSyntax : FunctionDeclarationSyntax
    {
        [NotNull] public IOperatorKeywordToken OperatorKeyword { get; }
        public override IIdentifierTokenPlace Name => throw new System.NotImplementedException();
        [NotNull] public IOperatorTokenPlace Operator { get; }
        [NotNull] public IRightArrowTokenPlace Arrow { get; }
        [NotNull] public ExpressionSyntax ReturnTypeExpression { get; }
        [NotNull] public SyntaxList<GenericConstraintSyntax> GenericConstraints { get; }

        public OperatorFunctionDeclarationSyntax(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] IOperatorKeywordToken operatorKeyword,
            [NotNull] IOperatorTokenPlace @operator,
            [NotNull] IOpenParenTokenPlace openParen,
            [NotNull] SeparatedListSyntax<ParameterSyntax> parameterList,
            [NotNull] ICloseParenTokenPlace closeParen,
            [NotNull] IRightArrowTokenPlace arrow,
            [NotNull] ExpressionSyntax returnTypeExpression,
            [NotNull] SyntaxList<GenericConstraintSyntax> genericConstraints,
            [CanBeNull] EffectsSyntax effects,
            [NotNull] SyntaxList<FunctionContractSyntax> contracts,
            [CanBeNull] BlockSyntax body,
            [CanBeNull] ISemicolonTokenPlace semicolon)
            : base(TextSpan.Covering(operatorKeyword.Span, @operator.Span),
            modifiers, openParen, parameterList, closeParen, effects, contracts, body, semicolon)
        {
            Requires.NotNull(nameof(modifiers), modifiers);
            Requires.NotNull(nameof(operatorKeyword), operatorKeyword);
            Requires.NotNull(nameof(@operator), @operator);
            Requires.NotNull(nameof(openParen), openParen);
            Requires.NotNull(nameof(parameterList), parameterList);
            Requires.NotNull(nameof(closeParen), closeParen);
            Requires.NotNull(nameof(arrow), arrow);
            Requires.NotNull(nameof(returnTypeExpression), returnTypeExpression);
            Requires.NotNull(nameof(genericConstraints), genericConstraints);
            OperatorKeyword = operatorKeyword;
            Operator = @operator;
            Arrow = arrow;
            ReturnTypeExpression = returnTypeExpression;
            GenericConstraints = genericConstraints;
        }
    }
}
