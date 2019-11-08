using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places
{
    public class VariablePlace : ValuePlace
    {
        public Variable Variable { get; }

        public VariablePlace(Variable variable, in TextSpan span)
            : base(span)
        {
            Variable = variable;
        }
    }
}
