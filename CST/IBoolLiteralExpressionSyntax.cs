namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface IBoolLiteralExpressionSyntax : ILiteralExpressionSyntax
    {
        bool Value { get; }
    }
}
