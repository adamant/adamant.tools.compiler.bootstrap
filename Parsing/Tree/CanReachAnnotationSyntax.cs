using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class CanReachAnnotationSyntax : Syntax, ICanReachAnnotationSyntax
    {
        public FixedList<IParameterNameSyntax> Parameters { get; }

        public CanReachAnnotationSyntax(TextSpan span, FixedList<IParameterNameSyntax> parameters)
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
