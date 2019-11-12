using Adamant.Tools.Compiler.Bootstrap.Core;
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

        public abstract override string ToString();
    }
}
