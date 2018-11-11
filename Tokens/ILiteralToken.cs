namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public interface ILiteralToken : IToken { }

    public partial interface IBooleanLiteralToken : ILiteralToken { }
    public partial interface IIntegerLiteralToken : ILiteralToken { }
    public partial interface IStringLiteralToken : ILiteralToken { }
    public partial interface INoneKeywordToken : ILiteralToken { }
    public partial interface IUninitializedKeywordToken : ILiteralToken { }
}
