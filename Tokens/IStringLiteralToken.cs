namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public partial interface IStringLiteralToken : IToken
    {
        string Value { get; }
    }
}
