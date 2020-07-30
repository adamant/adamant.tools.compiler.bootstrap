using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    [Closed(
        typeof(ICanReachAnnotationSyntax),
        typeof(IReachableFromAnnotationSyntax))]
    public interface IReachabilityAnnotationSyntax : ISyntax
    {
    }
}
