using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class Dereference : Place
    {
        public readonly Place DereferencedValue;

        public Dereference(Place dereferencedValue, TextSpan span)
            : base(span)
        {
            DereferencedValue = dereferencedValue;
        }

        public override VariableNumber CoreVariable()
        {
            return DereferencedValue.CoreVariable();
        }

        public override string ToString()
        {
            return $"^{DereferencedValue}";
        }
    }
}
