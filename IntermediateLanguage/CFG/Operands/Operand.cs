using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands
{
    public abstract class Operand
    {
        public TextSpan Span { get; }

        protected Operand(in TextSpan span)
        {
            Span = span;
        }
    }
}
