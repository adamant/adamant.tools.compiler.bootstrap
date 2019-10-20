namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IArgumentSyntax : ISyntax
    {
        ref IExpressionSyntax Expression { get; }
    }
}
