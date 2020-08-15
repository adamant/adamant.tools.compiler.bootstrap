namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    /// <summary>
    /// i.e. `mut exp`. A borrow expression causes a borrow from a variable. That
    /// is, the result is a borrow of the value referenced. Note that borrow expressions
    /// don't apply to value types since they are passed by move, copy, or reference.
    /// </summary>
    public partial interface IBorrowExpressionSyntax
    {
    }
}
