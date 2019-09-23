using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    /// <summary>
    /// An operand is something that can be used as the operand of an operator
    /// or and argument to a call.
    /// </summary>
    [Closed(
        typeof(Dereference),
        typeof(Constant),
        typeof(VariableReference))]
    public interface IOperand : IValue
    {
    }
}
