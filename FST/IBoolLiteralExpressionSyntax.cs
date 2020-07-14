namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface IBoolLiteralExpressionSyntax : ILiteralExpressionSyntax
    {
        bool Value { get; }
    }
}
