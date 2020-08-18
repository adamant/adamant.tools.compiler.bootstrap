using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class ReachableFromAnnotationSyntax : Syntax, IReachableFromAnnotationSyntax
    {
        public FixedList<INameOrSelfExpressionSyntax> ReachableFrom { get; }

        public ReachableFromAnnotationSyntax(TextSpan span, FixedList<INameOrSelfExpressionSyntax> reachableFrom)
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
