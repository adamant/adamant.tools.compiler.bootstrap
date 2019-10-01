namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IBoolLiteralExpressionSyntax : ILiteralExpressionSyntax
    {
        bool Value { get; }
    }
}
