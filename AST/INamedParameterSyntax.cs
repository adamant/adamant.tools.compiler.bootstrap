namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface INamedParameterSyntax : IParameterSyntax, IConstructorParameterSyntax, IMethodParameterSyntax
    {
        ITypeSyntax TypeSyntax { get; }
        IExpressionSyntax? DefaultValue { get; }
    }
}
