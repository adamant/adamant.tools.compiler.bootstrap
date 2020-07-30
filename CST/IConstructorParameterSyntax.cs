using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    [Closed(
        typeof(INamedParameterSyntax),
        typeof(IFieldParameterSyntax))]
    public interface IConstructorParameterSyntax : IParameterSyntax
    {
    }
}
