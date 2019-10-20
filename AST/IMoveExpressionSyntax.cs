namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// i.e. `move name`. A move takes ownership of something from a variable
    /// </summary>
    public interface IMoveExpressionSyntax : IExpressionSyntax
    {
        INameExpressionSyntax Expression { get; }
    }
}
