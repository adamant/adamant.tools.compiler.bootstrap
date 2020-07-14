namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface INamedParameterSyntax : IParameterSyntax, IConstructorParameterSyntax
    {
        ITypeSyntax TypeSyntax { get; }
        IExpressionSyntax? DefaultValue { get; }
    }
}
