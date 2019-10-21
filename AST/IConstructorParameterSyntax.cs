using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(INamedParameterSyntax),
        typeof(IFieldParameterSyntax))]
    public interface IConstructorParameterSyntax : IParameterSyntax
    {
    }
}
