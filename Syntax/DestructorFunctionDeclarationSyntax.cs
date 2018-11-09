using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class DestructorFunctionDeclarationSyntax : FunctionDeclarationSyntax
    {
        [NotNull] public DeleteKeywordToken DeleteKeyword { get; }
        public override IIdentifierToken Name => throw new System.NotImplementedException();

        public DestructorFunctionDeclarationSyntax(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] DeleteKeywordToken deleteKeyword,
            [NotNull] IOpenParenToken openParen,
            [NotNull] SeparatedListSyntax<ParameterSyntax> parameters,
            [NotNull] ICloseParenToken closeParen,
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
