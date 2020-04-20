namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// i.e. `mut exp`. A mutable expression marks an expression whose type
    /// should be treated as mutable rather than defaulting to read-only.
    /// </summary>
    public interface IMutableExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax Referent { get; }
    }
}
