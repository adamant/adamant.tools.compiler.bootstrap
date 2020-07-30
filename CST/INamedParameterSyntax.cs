namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface INamedParameterSyntax : IParameterSyntax, IConstructorParameterSyntax
    {
        ITypeSyntax TypeSyntax { get; }
        IExpressionSyntax? DefaultValue { get; }
    }
}
