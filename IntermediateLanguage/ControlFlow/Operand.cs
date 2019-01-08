using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    /// <summary>
    /// An operand is something that can be used as the operand of an operator
    /// or and argument to a call.
    /// </summary>
    public abstract class Operand : Value
    {
        protected Operand(TextSpan span)
            : base(span)
        {
        }
    }
}
