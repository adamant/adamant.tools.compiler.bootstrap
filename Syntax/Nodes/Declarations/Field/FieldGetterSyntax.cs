using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Field
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
