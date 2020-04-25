using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IReachableFromAnnotationSyntax : IReachabilityAnnotationSyntax
    {
        FixedList<INameExpressionSyntax> ReachableFrom { get; }
    }
}
