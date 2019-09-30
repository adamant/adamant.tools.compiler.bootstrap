namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface INamedParameterSyntax : IParameterSyntax
    {
        TypeSyntax TypeSyntax { get; }
        ExpressionSyntax DefaultValue { get; }
    }
}
