namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface IStringLiteralExpressionSyntax : ILiteralExpressionSyntax
    {
        string Value { get; }
    }
}
