using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class NamedFunctionDeclarationSyntax : FunctionDeclarationSyntax
    {
        [NotNull] public IFunctionKeywordToken FunctionKeyword { get; }
        [NotNull] public override IIdentifierTokenPlace Name { get; }
        [CanBeNull] public GenericParametersSyntax GenericParameters { get; }
        [NotNull] public IRightArrowTokenPlace Arrow { get; }
        [NotNull] public ExpressionSyntax ReturnTypeExpression { get; }
        [NotNull] public SyntaxList<GenericConstraintSyntax> GenericConstraints { get; }

        public NamedFunctionDeclarationSyntax(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] IFunctionKeywordToken functionKeyword,
            [NotNull] IIdentifierTokenPlace name,
            [CanBeNull] GenericParametersSyntax genericParameters,
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
            : base(TextSpan.Covering(functionKeyword.Span, name.Span), modifiers,
            openParen, parameterList, closeParen, effects, contracts, body, semicolon)
        {
            Requires.NotNull(nameof(modifiers), modifiers);
            Requires.NotNull(nameof(functionKeyword), functionKeyword);
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(openParen), openParen);
            Requires.NotNull(nameof(parameterList), parameterList);
            Requires.NotNull(nameof(genericConstraints), genericConstraints);
            Requires.NotNull(nameof(closeParen), closeParen);
            Requires.NotNull(nameof(arrow), arrow);
            Requires.NotNull(nameof(returnTypeExpression), returnTypeExpression);
            FunctionKeyword = functionKeyword;
            Name = name;
            GenericParameters = genericParameters;
            Arrow = arrow;
            ReturnTypeExpression = returnTypeExpression;
            GenericConstraints = genericConstraints;
        }
    }
}
