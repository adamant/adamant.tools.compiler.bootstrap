namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// i.e. `mut name`. A mutable expression borrows mutably from a variable
    /// </summary>
    public interface IMutableExpressionSyntax : IExpressionSyntax
    {
        INameExpressionSyntax Expression { get; }
    }
}
