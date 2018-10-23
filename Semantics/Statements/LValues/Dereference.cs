using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Statements.LValues
{
    public class Dereference : LValue
    {
        [NotNull] public readonly LValue DereferencedValue;

        public Dereference([NotNull] LValue dereferencedValue)
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
