namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    // TODO I think this exists from the days when `mut` and `move` were treated as special parts of the argument. Perhaps remove this.
    public partial interface IArgumentSyntax : ISyntax
    {
        ref IExpressionSyntax Expression { get; }
    }
}
