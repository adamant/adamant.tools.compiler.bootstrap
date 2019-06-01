using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class VariableReference : Place
    {
        public readonly VariableNumber VariableNumber;

        public VariableReference(VariableNumber variableNumber, TextSpan span)
            : base(span)
        {
            VariableNumber = variableNumber;
        }

        public override string ToString()
        {
            return $"%{VariableNumber}";
        }

        public override VariableNumber CoreVariable()
        {
            return VariableNumber;
        }
    }
}
