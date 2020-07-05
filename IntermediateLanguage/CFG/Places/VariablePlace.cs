using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places
{
    public class VariablePlace : Place
    {
        public Variable Variable { get; }

        public VariablePlace(Variable variable, TextSpan span)
            : base(span)
        {
            Variable = variable;
        }

        public override Operand ToOperand(TextSpan span)
        {
            // TODO is this the correct value semantics?
            return new VariableReference(Variable, ValueSemantics.Copy, span);
        }

        public override string ToString()
        {
            return Variable.ToString();
        }
    }
}
