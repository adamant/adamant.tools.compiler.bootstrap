using Adamant.Tools.Compiler.Bootstrap.Core;

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

        public override string ToString()
        {
            return Variable.ToString();
        }
    }
}
