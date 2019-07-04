using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class Dereference : Value, IPlace, IOperand
    {
        public readonly IPlace DereferencedValue;

        public Dereference(IPlace dereferencedValue, TextSpan span)
            : base(span)
        {
            DereferencedValue = dereferencedValue;
        }

        public Variable CoreVariable()
        {
            return DereferencedValue.CoreVariable();
        }

        public override string ToString()
        {
            return $"^{DereferencedValue}";
        }
    }
}
