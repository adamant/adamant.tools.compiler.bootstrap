using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    public class DeleteStatement : SimpleStatement
    {
        public readonly int VariableNumber;
        public readonly TextSpan Span;

        public DeleteStatement(int variableNumber, TextSpan span)
        {
            VariableNumber = variableNumber;
            Span = span;
        }
    }
}
