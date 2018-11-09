namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public interface IMemberNameTokenPlace : IKeywordTokenPlace { }

    public partial interface IIdentifierTokenPlace : IMemberNameTokenPlace { }
    public partial interface INewKeywordTokenPlace : IMemberNameTokenPlace { }
    public partial interface IInitKeywordTokenPlace : IMemberNameTokenPlace { }
    public partial interface IDeleteKeywordTokenPlace : IMemberNameTokenPlace { }
}
