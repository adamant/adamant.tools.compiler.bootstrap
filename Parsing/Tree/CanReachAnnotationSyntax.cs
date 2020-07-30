using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class CanReachAnnotationSyntax : Syntax, ICanReachAnnotationSyntax
    {
        public FixedList<INameExpressionSyntax> CanReach { get; }

        public CanReachAnnotationSyntax(TextSpan span, FixedList<INameExpressionSyntax> canReach)
            : base(span)
        {
            CanReach = canReach;
        }

        public override string ToString()
        {
            return $"~> {string.Join(", ", CanReach)}";
        }
    }
}
