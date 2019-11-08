using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places
{
    public abstract class Place
    {
        public TextSpan Span { get; }

        protected Place(TextSpan span)
        {
            Span = span;
        }
    }
}
