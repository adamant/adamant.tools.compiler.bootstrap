namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public interface ILifetimeNameToken : IToken
    {
    }

    public partial interface IOwnedKeywordToken : ILifetimeNameToken
    {
    }

    public partial interface IRefKeywordToken : ILifetimeNameToken
    {
    }
}
