using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class DeleteStatement : ExpressionStatement
    {
        public readonly int VariableNumber;
        public readonly TextSpan Span;

        public DeleteStatement(int variableNumber, TextSpan span)
        {
            VariableNumber = variableNumber;
            Span = span;
        }

        // Useful for debugging
        public override string ToString()
        {
            return $"delete {VariableNumber} // at {Span}";
        }
    }
}
