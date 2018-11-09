namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public interface ILifetimeNameTokenPlace : ITokenPlace
    {
    }

    public partial interface IOwnedKeywordTokenPlace : ILifetimeNameTokenPlace { }
    public partial interface IRefKeywordTokenPlace : ILifetimeNameTokenPlace { }
}
