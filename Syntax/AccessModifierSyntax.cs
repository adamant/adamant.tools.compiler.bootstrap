namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    //public class AccessModifierSyntax : ModifierSyntax
    //{
    //    [NotNull] public IAccessModifierToken Token { get; }
    //    public AccessModifier Modifier { get; }

    //    public AccessModifierSyntax([NotNull] IAccessModifierToken token)
    //    {
    //        Requires.NotNull(nameof(token), token);
    //        Token = token;
    //        switch (Token)
    //        {
    //            case IMissingToken _: // To avoid later errors, if the modifer is missing, we treat it as public
    //            case IPublicKeywordToken _:
    //                Modifier = AccessModifier.Public;
    //                break;
    //            case IProtectedKeywordToken _:
    //                Modifier = AccessModifier.Protected;
    //                break;
    //            case IPrivateKeywordToken _:
    //                Modifier = AccessModifier.Private;
    //                break;
    //            case IInternalKeywordToken _:
    //                Modifier = AccessModifier.Internal;
    //                break;
    //            default:
    //                // must be of those type
    //                Requires.That(nameof(token), false);
    //                break;
    //        }
    //    }

    //    public override IEnumerable<ITokenPlace> Tokens()
    //    {
    //        yield return Token;
    //    }
    //}
}
