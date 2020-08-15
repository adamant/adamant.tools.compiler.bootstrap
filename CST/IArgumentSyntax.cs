namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    /// <remarks>
    /// This is needed because it is not possible to put <code>ref IExpressionSyntax</code>
    /// directly into a list.
    ///
    /// TODO remove type once expression is immutable
    /// </remarks>
    public partial interface IArgumentSyntax
    {
    }
}
