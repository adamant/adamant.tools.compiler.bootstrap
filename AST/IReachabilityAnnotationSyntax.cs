using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(ICanReachAnnotationSyntax),
        typeof(IReachableFromAnnotationSyntax))]
    public interface IReachabilityAnnotationSyntax : ISyntax
    {
    }
}
