using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class FieldGetterSyntax : SyntaxNode
    {
        [CanBeNull] public PublicKeywordToken PublicKeyword { get; }
        [CanBeNull] public GetKeywordToken GetKeyword { get; }

        public FieldGetterSyntax(
            [CanBeNull] PublicKeywordToken publicKeyword,
            [CanBeNull] GetKeywordToken getKeyword)
        {
            PublicKeyword = publicKeyword;
            GetKeyword = getKeyword;
        }
    }
}
