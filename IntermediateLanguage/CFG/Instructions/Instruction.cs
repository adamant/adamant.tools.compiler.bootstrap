using Adamant.Tools.Compiler.Bootstrap.Core;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
{
    [Closed(
        typeof(CallInstruction),
        typeof(CallVirtualInstruction),
        typeof(InstructionWithResult))]
    public abstract class Instruction
    {
        public TextSpan Span { get; }
        public Scope Scope { get; }

        protected Instruction(TextSpan span, Scope scope)
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
