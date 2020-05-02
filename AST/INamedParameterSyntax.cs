namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface INamedParameterSyntax : IParameterSyntax, IConstructorParameterSyntax
    {
        ITypeSyntax TypeSyntax { get; }
        IExpressionSyntax? DefaultValue { get; }
    }
}
