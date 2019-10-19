using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    [Closed(
        typeof(IOwnedKeywordToken),
        //typeof(IRefKeywordToken),
        typeof(IForeverKeywordToken),
        typeof(IIdentifierToken))]
    public interface ILifetimeNameToken : IEssentialToken { }

    public partial interface IOwnedKeywordToken : ILifetimeNameToken { }
    //public partial interface IRefKeywordToken : ILifetimeNameToken { }
    public partial interface IForeverKeywordToken : ILifetimeNameToken { }
    public partial interface IIdentifierToken : ILifetimeNameToken { }
}
