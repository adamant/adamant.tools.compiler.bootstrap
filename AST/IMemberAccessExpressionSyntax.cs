namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IMemberAccessExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax? Expression { get; }
        AccessOperator AccessOperator { get; }
        NameSyntax Member { get; }
    }
}
