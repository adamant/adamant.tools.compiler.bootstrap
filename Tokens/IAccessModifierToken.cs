using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    [Closed(
        typeof(IPublishedKeywordToken),
        typeof(IPublicKeywordToken)
        )]
    public partial interface IAccessModifierToken : IKeywordToken { }

    public partial interface IPublishedKeywordToken : IAccessModifierToken { }
    public partial interface IPublicKeywordToken : IAccessModifierToken { }
}
