using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    public abstract class NumericType : SimpleType
    {
        protected NumericType([NotNull] string name)
            : base(name)
        {
        }
    }
}
