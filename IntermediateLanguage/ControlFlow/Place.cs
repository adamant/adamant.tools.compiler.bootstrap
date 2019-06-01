using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public abstract class Place : Operand
    {
        protected Place(TextSpan span)
            : base(span)
        {
        }

        public abstract VariableNumber CoreVariable();
    }
}
