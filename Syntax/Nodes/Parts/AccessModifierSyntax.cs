using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts
{
    public class AccessModifierSyntax : SyntaxNode
    {
        [CanBeNull]
        public KeywordToken Keyword { get; }

        public AccessModifier Modifier { get; }

        public AccessModifierSyntax([CanBeNull] KeywordToken keyword)
        {
            Keyword = keyword;
            switch (Keyword)
            {
                case null: // To avoid later errors, if the modifer is missing, we treat it as public
                case PublicKeywordToken _:
                    Modifier = AccessModifier.Public;
                    break;
                case ProtectedKeywordToken _:
                    Modifier = AccessModifier.Protected;
                    break;
                case PrivateKeywordToken _:
                    Modifier = AccessModifier.Private;
                    break;
                default:
                    // must be of those type
                    Requires.That(nameof(keyword), false);
                    break;
            }
        }
    }
}
