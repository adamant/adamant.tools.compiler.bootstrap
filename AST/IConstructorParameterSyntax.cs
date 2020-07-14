using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    [Closed(
        typeof(INamedParameterSyntax),
        typeof(IFieldParameterSyntax))]
    public interface IConstructorParameterSyntax : IParameterSyntax
    {
    }
}
