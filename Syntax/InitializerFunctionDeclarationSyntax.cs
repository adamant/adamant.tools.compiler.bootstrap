using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class InitializerFunctionDeclarationSyntax : FunctionDeclarationSyntax
    {
        [NotNull] public IInitKeywordToken InitKeyword { get; }
        [CanBeNull] public override IIdentifierTokenPlace Name { get; }
        [CanBeNull] public GenericParametersSyntax GenericParameters { get; }
        [NotNull] public SyntaxList<GenericConstraintSyntax> GenericConstraints { get; }

        public InitializerFunctionDeclarationSyntax(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] IInitKeywordToken initKeyword,
            [CanBeNull] IIdentifierToken name,
            [CanBeNull] GenericParametersSyntax genericParameters,
            [NotNull] IOpenParenTokenPlace openParen,
            [NotNull] SeparatedListSyntax<ParameterSyntax> parameterList,
            [NotNull] ICloseParenTokenPlace closeParen,
            [NotNull] SyntaxList<GenericConstraintSyntax> genericConstraints,
            [CanBeNull] EffectsSyntax effects,
            [NotNull] SyntaxList<FunctionContractSyntax> contracts,
            [NotNull] BlockSyntax body)
            : base(TextSpan.Covering(initKeyword.Span, name?.Span), modifiers,
            openParen, parameterList, closeParen, effects, contracts, body, null)
        {
            Requires.NotNull(nameof(modifiers), modifiers);
            Requires.NotNull(nameof(initKeyword), initKeyword);
            Requires.NotNull(nameof(openParen), openParen);
            Requires.NotNull(nameof(parameterList), parameterList);
            Requires.NotNull(nameof(closeParen), closeParen);
            Requires.NotNull(nameof(genericConstraints), genericConstraints);
            Requires.NotNull(nameof(contracts), contracts);
            Requires.NotNull(nameof(body), body);
            InitKeyword = initKeyword;
            Name = name;
            GenericParameters = genericParameters;
            GenericConstraints = genericConstraints;
        }
    }
}
