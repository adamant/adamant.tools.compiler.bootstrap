using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(INamedParameterSyntax),
        typeof(ISelfParameterSyntax))]
    public interface IMethodParameterSyntax : IParameterSyntax
    {
    }
}
