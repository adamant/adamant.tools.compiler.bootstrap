using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    [Closed(
        typeof(IIdentifierToken))]
    public interface IIdentifierOrUnderscoreToken : IEssentialToken
    {
        string Value { get; }
    }

    public partial interface IIdentifierToken : IIdentifierOrUnderscoreToken { }
    //public partial interface IUnderscoreKeywordToken : IIdentifierOrUnderscoreToken { }

    //internal partial class UnderscoreKeywordToken
    //{
    //    public string Value => "_";
    //}
}
