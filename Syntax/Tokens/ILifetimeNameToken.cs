namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
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
