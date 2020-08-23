using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class ReachableFromAnnotation : AbstractSyntax, IReachableFromAnnotation
    {
        public FixedList<IParameterName> Parameters { get; }

        public ReachableFromAnnotation(TextSpan span, FixedList<IParameterName> parameters)
            : base(span)
        {
            Parameters = parameters;
        }

        public override string ToString()
        {
            return $"<~ {string.Join(", ", Parameters)}";
        }
    }
}
