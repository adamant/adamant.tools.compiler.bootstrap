namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IBreakExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax? Value { get; }
    }
}
