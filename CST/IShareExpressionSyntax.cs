namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    /// <summary>
    /// A bare variable of a reference type is generally a share of that reference.
    /// Share expressions are inserted into the AST where needed to reflect this.
    /// </summary>
    public partial interface IShareExpressionSyntax
    {
        ref IExpressionSyntax Referent { get; }
    }
}
