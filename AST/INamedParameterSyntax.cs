namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface INamedParameterSyntax : IParameterSyntax
    {
        ITypeSyntax TypeSyntax { get; }
        ExpressionSyntax DefaultValue { get; }
    }
}
