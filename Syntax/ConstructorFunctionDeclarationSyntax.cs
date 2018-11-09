using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ConstructorFunctionDeclarationSyntax : FunctionDeclarationSyntax
    {
        [NotNull] public INewKeywordToken NewKeyword { get; }
        [CanBeNull] public override IIdentifierTokenPlace Name { get; }
        [CanBeNull] public GenericParametersSyntax GenericParameters { get; }
        [NotNull] public SyntaxList<GenericConstraintSyntax> GenericConstraints { get; }

        public ConstructorFunctionDeclarationSyntax(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] INewKeywordToken newKeyword,
            [CanBeNull] IIdentifierToken name,
            [CanBeNull] GenericParametersSyntax genericParameters,
            [NotNull] IOpenParenTokenPlace openParen,
            [NotNull] SeparatedListSyntax<ParameterSyntax> parameterList,
            [NotNull] ICloseParenTokenPlace closeParen,
            [NotNull] SyntaxList<GenericConstraintSyntax> genericConstraints,
            [CanBeNull] EffectsSyntax effects,
            [NotNull] SyntaxList<FunctionContractSyntax> contracts,
            [NotNull] BlockSyntax body)
            : base(TextSpan.Covering(newKeyword.Span, name?.Span), modifiers,
            openParen, parameterList, closeParen, effects, contracts, body, null)
        {
            Requires.NotNull(nameof(modifiers), modifiers);
            Requires.NotNull(nameof(newKeyword), newKeyword);
            Requires.NotNull(nameof(openParen), openParen);
            Requires.NotNull(nameof(parameterList), parameterList);
            Requires.NotNull(nameof(closeParen), closeParen);
            Requires.NotNull(nameof(genericConstraints), genericConstraints);
            Requires.NotNull(nameof(contracts), contracts);
            Requires.NotNull(nameof(body), body);
            NewKeyword = newKeyword;
            Name = name;
            GenericParameters = genericParameters;
            GenericConstraints = genericConstraints;
        }
    }
}
