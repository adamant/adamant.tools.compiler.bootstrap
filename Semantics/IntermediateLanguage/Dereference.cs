using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    public class Dereference : Place
    {
        [NotNull] public readonly Place DereferencedValue;

        public Dereference([NotNull] Place dereferencedValue)
        {
            Requires.NotNull(nameof(dereferencedValue), dereferencedValue);
            DereferencedValue = dereferencedValue;
        }

        public override int CoreVariable()
        {
            return DereferencedValue.CoreVariable();
        }

        public override string ToString()
        {
            return $"^{DereferencedValue}";
        }
    }
}
