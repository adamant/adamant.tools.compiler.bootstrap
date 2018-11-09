using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class DestructorFunctionDeclarationSyntax : FunctionDeclarationSyntax
    {
        [NotNull] public IDeleteKeywordToken DeleteKeyword { get; }
        public override IIdentifierTokenPlace Name => throw new System.NotImplementedException();

        public DestructorFunctionDeclarationSyntax(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] IDeleteKeywordToken deleteKeyword,
            [NotNull] IOpenParenTokenPlace openParen,
            [NotNull] SeparatedListSyntax<ParameterSyntax> parameters,
            [NotNull] ICloseParenTokenPlace closeParen,
            [CanBeNull] EffectsSyntax effects,
            [NotNull] SyntaxList<FunctionContractSyntax> contracts,
            [NotNull] BlockSyntax body)
            : base(deleteKeyword.Span, modifiers, openParen, parameters, closeParen,
                effects, contracts, body, null)
        {
            Requires.NotNull(nameof(deleteKeyword), deleteKeyword);
            Requires.NotNull(nameof(body), body);
            DeleteKeyword = deleteKeyword;
        }
    }
}
