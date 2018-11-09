namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public interface ITriviaToken : IToken { }

    public partial interface ICommentToken : ITriviaToken { }
    public partial interface IWhitespaceToken : ITriviaToken { }
    public partial interface IUnexpectedToken : ITriviaToken { }
}
