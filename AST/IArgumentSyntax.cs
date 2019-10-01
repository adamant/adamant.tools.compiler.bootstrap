namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IArgumentSyntax : ISyntax
    {
        IExpressionSyntax Value { get; }
        ref IExpressionSyntax ValueRef { get; }
    }
}
