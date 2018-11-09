namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public interface IMemberNameToken : IKeywordToken { }

    public partial interface IIdentifierToken : IMemberNameToken { }
    public partial interface INewKeywordToken : IMemberNameToken { }
    public partial interface IInitKeywordToken : IMemberNameToken { }
    public partial interface IDeleteKeywordToken : IMemberNameToken { }
}
