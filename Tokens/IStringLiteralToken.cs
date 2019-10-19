namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public partial interface IStringLiteralToken : IEssentialToken
    {
        string Value { get; }
    }
}
