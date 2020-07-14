using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.FST;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class ReachableFromAnnotationSyntax : Syntax, IReachableFromAnnotationSyntax
    {
        public FixedList<INameExpressionSyntax> ReachableFrom { get; }

        public ReachableFromAnnotationSyntax(TextSpan span, FixedList<INameExpressionSyntax> reachableFrom)
            : base(span)
        {
            ReachableFrom = reachableFrom;
        }

        public override string ToString()
        {
            return $"<~ {string.Join(", ", ReachableFrom)}";
        }
    }
}
