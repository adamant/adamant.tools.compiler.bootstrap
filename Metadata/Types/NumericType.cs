using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public abstract class NumericType : SimpleType
    {
        protected NumericType([NotNull] string name)
            : base(name)
        {
        }
    }
}
