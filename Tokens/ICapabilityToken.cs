using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    [Closed(
        typeof(IOwnedKeywordToken),
        typeof(IIsolatedKeywordToken),
        typeof(IHeldKeywordToken),
        typeof(IMutableKeywordToken),
        typeof(IIdKeywordToken))]
    public interface ICapabilityToken : IKeywordToken { }

    public partial interface IOwnedKeywordToken : ICapabilityToken { }
    public partial interface IIsolatedKeywordToken : ICapabilityToken { }
    public partial interface IHeldKeywordToken : ICapabilityToken { }
    public partial interface IMutableKeywordToken : ICapabilityToken { }
    public partial interface IIdKeywordToken : ICapabilityToken { }
}
