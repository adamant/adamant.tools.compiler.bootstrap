namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface IStringLiteralExpressionSyntax : ILiteralExpressionSyntax
    {
        string Value { get; }
    }
}
