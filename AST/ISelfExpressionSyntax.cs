namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface ISelfExpressionSyntax : IExpressionSyntax
    {
        bool IsImplicit { get; }
    }
}
