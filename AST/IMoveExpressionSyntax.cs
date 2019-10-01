namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IMoveExpressionSyntax : IExpressionSyntax
    {
        IExpressionSyntax Expression { get; }
    }
}
