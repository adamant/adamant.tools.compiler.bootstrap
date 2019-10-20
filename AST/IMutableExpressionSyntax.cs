namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// i.e. `mut exp`. A mutable expression borrows mutably from a variable or
    /// promotes an implicitly mutable expression to mutable
    /// </summary>
    public interface IMutableExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax Referent { get; }
    }
}
