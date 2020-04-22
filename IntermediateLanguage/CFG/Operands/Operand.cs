using Adamant.Tools.Compiler.Bootstrap.Core;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands
{
    [Closed(
        typeof(VariableReference))]
    public abstract class Operand
    {
        public TextSpan Span { get; }

        protected Operand(in TextSpan span)
        {
            Span = span;
        }
    }
}
