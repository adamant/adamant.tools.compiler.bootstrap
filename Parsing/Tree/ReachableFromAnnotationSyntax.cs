using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class ReachableFromAnnotationSyntax : Syntax, IReachableFromAnnotationSyntax
    {
        public FixedList<IParameterNameSyntax> Parameters { get; }

        public ReachableFromAnnotationSyntax(TextSpan span, FixedList<IParameterNameSyntax> parameters)
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
