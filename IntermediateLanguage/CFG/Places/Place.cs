using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places
{
    [Closed(
        typeof(VariablePlace),
        typeof(FieldPlace))]
    public abstract class Place
    {
        public TextSpan Span { get; }

        protected Place(TextSpan span)
        {
            Span = span;
        }

        public abstract Operand ToOperand(TextSpan span);

        public abstract override string ToString();
    }
}
