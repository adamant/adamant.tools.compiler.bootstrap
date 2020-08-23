using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class ReachabilityAnnotations : AbstractSyntax, IReachabilityAnnotations
    {
        public IReachableFromAnnotation? ReachableFromAnnotation { get; }
        public ICanReachAnnotation? CanReachAnnotation { get; }

        public ReachabilityAnnotations(
            TextSpan span,
            IReachableFromAnnotation? reachableFromAnnotation,
            ICanReachAnnotation? canReachAnnotation)
            : base(span)
        {
            ReachableFromAnnotation = reachableFromAnnotation;
            CanReachAnnotation = canReachAnnotation;
        }

        public override string ToString()
        {
            var result = "";
            if (ReachableFromAnnotation != null) result += $" {ReachableFromAnnotation}";
            if (CanReachAnnotation != null) result += $" {CanReachAnnotation}";
            return result;
        }
    }
}
