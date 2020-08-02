using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    [Closed(
        typeof(INamedParameterSyntax),
        typeof(IFieldParameterSyntax))]
    public partial interface IConstructorParameterSyntax : IParameterSyntax
    {
    }
}
