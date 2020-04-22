using Adamant.Tools.Compiler.Bootstrap.Core;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.TerminatorInstructions
{
    /// <summary>
    /// An instruction that terminates a block
    /// </summary>
    [Closed(
        typeof(GotoInstruction),
        typeof(IfInstruction),
        typeof(ReturnValueInstruction),
        typeof(ReturnVoidInstruction))]
    public abstract class TerminatorInstruction
    {
        public TextSpan Span { get; }
        public Scope Scope { get; }

        protected TerminatorInstruction(TextSpan span, Scope scope)
        {
            Span = span;
            Scope = scope;
        }

        public override string ToString()
        {
            return ToInstructionString() + ContextCommentString();
        }

        public abstract string ToInstructionString();

        public virtual string ContextCommentString()
        {
            return $" // at {Span} in {Scope}";
        }
    }
}
