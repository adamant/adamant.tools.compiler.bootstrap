using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface ICanReachAnnotationSyntax : IReachabilityAnnotationSyntax
    {
        FixedList<INameExpressionSyntax> CanReach { get; }
    }
}
