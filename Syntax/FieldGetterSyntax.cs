using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class FieldGetterSyntax : NonTerminal
    {
        [CanBeNull] public IPublicKeywordToken PublicKeyword { get; }
        [CanBeNull] public IGetKeywordToken GetKeyword { get; }

        public FieldGetterSyntax(
            [CanBeNull] IPublicKeywordToken publicKeyword,
            [CanBeNull] IGetKeywordToken getKeyword)
        {
            PublicKeyword = publicKeyword;
            GetKeyword = getKeyword;
        }
    }
}
