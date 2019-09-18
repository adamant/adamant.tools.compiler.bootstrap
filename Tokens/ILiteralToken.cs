using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    [Closed(
        typeof(IBooleanLiteralToken),
        typeof(IIntegerLiteralToken),
        typeof(IStringLiteralToken),
        typeof(INoneKeywordToken)
        //typeof(IUninitializedKeywordToken)
        )]
    public interface ILiteralToken : IToken { }

    public partial interface IBooleanLiteralToken : ILiteralToken { }
    public partial interface IIntegerLiteralToken : ILiteralToken { }
    public partial interface IStringLiteralToken : ILiteralToken { }
    public partial interface INoneKeywordToken : ILiteralToken { }
    //public partial interface IUninitializedKeywordToken : ILiteralToken { }
}
