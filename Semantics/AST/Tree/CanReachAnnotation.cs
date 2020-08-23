using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class CanReachAnnotation : AbstractSyntax, ICanReachAnnotation
    {
        public FixedList<IParameterName> Parameters { get; }

        public CanReachAnnotation(TextSpan span, FixedList<IParameterName> parameters)
            : base(span)
        {
            Parameters = parameters;
        }

        public override string ToString()
        {
            return $"~> {string.Join(", ", Parameters)}";
        }
    }
}
