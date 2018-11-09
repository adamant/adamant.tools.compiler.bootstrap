using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class AccessModifierSyntax : ModifierSyntax
    {
        [NotNull] public IAccessModifierToken Token { get; }
        public AccessModifier Modifier { get; }

        public AccessModifierSyntax([NotNull] IAccessModifierToken token)
        {
            Requires.NotNull(nameof(token), token);
            Token = token;
            switch (Token)
            {
                case MissingToken _: // To avoid later errors, if the modifer is missing, we treat it as public
                case PublicKeywordToken _:
                    Modifier = AccessModifier.Public;
                    break;
                case ProtectedKeywordToken _:
                    Modifier = AccessModifier.Protected;
                    break;
                case PrivateKeywordToken _:
                    Modifier = AccessModifier.Private;
                    break;
                case InternalKeywordToken _:
                    Modifier = AccessModifier.Internal;
                    break;
                default:
                    // must be of those type
                    Requires.That(nameof(token), false);
                    break;
            }
        }

        public override IEnumerable<IToken> Tokens()
        {
            yield return Token;
        }
    }
}
