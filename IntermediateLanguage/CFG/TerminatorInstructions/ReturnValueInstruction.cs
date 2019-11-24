using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.TerminatorInstructions
{
    public class ReturnValueInstruction : TerminatorInstruction
    {
        protected ReturnValueInstruction(TextSpan span, Scope scope)
            : base(span, scope) { }
    }
}
