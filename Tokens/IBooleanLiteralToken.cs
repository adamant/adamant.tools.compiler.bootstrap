namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public partial interface IBooleanLiteralToken : IKeywordToken
    {
        bool Value { get; }
    }

    public partial interface ITrueKeywordToken : IBooleanLiteralToken { }
    public partial interface IFalseKeywordToken : IBooleanLiteralToken { }
}
