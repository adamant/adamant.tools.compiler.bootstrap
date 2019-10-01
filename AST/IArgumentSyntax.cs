namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// Keeping around for the moment in case `mut`, or `move` should be on the
    /// argument not the expression.
    /// </summary>
    public interface IArgumentSyntax : ISyntax
    {
        ref IExpressionSyntax Value { get; }
    }
}
