using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class ReachabilityAnnotationsSyntax : Syntax, IReachabilityAnnotationsSyntax
    {
        public IReachableFromAnnotationSyntax? ReachableFromAnnotation { get; }
        public ICanReachAnnotationSyntax? CanReachAnnotation { get; }

        public ReachabilityAnnotationsSyntax(
            TextSpan span,
            IReachableFromAnnotationSyntax? reachableFromAnnotation,
            ICanReachAnnotationSyntax? canReachAnnotation)
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
