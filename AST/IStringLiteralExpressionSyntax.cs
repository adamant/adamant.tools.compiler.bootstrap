namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IStringLiteralExpressionSyntax : ILiteralExpressionSyntax
    {
        string Value { get; }
    }
}
