using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.TerminatorInstructions
{
    public class TerminatorInstruction
    {
        public TextSpan Span { get; }
        public Scope Scope { get; }

        protected TerminatorInstruction(TextSpan span, Scope scope)
        {
            Span = span;
            Scope = scope;
        }
    }
}
