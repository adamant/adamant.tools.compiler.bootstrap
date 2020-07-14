using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    [Closed(
        typeof(ICanReachAnnotationSyntax),
        typeof(IReachableFromAnnotationSyntax))]
    public interface IReachabilityAnnotationSyntax : ISyntax
    {
    }
}
