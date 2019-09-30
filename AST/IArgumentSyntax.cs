namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IArgumentSyntax : ISyntax
    {
        ExpressionSyntax Value { get; }
        ref ExpressionSyntax ValueRef { get; }
    }
}
