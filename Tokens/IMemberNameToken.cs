using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    [Closed(
        typeof(IIdentifierToken),
        typeof(INewKeywordToken),
        typeof(IInitKeywordToken),
        typeof(IDeleteKeywordToken))]
    public interface IMemberNameToken : IKeywordToken { }

    [Closed(
        typeof(IBareIdentifierToken),
        typeof(IEscapedIdentifierToken))]
    public partial interface IIdentifierToken : IMemberNameToken { }
    public partial interface INewKeywordToken : IMemberNameToken { }
    public partial interface IInitKeywordToken : IMemberNameToken { }
    public partial interface IDeleteKeywordToken : IMemberNameToken { }
}
